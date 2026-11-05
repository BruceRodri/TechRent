import re,urllib.request,urllib.parse,http.cookiejar,threading,subprocess,time
from pathlib import Path
jar=http.cookiejar.CookieJar()
opener=urllib.request.build_opener(urllib.request.HTTPCookieProcessor(jar))
url='http://127.0.0.1:5122'
html=opener.open(url+'/Identity/Account/Login',timeout=10).read().decode()
token=re.search(r'name="__RequestVerificationToken"[^>]*value="([^"]+)',html).group(1)
data=urllib.parse.urlencode({'Input.Email':'admin@techrent.com','Input.Password':'Admin@123','__RequestVerificationToken':token}).encode()
opener.open(url+'/Identity/Account/Login',data=data,timeout=10).read()
results={}
ready=[threading.Event(),threading.Event()]
def listen(port,index):
 try:
  response=opener.open(f'http://127.0.0.1:{port}/eventos/reservas',timeout=20)
  instance=response.headers.get('X-TechRent-Instance')
  for raw in response:
   line=raw.decode().strip()
   if line.startswith(': conectado'): ready[index].set()
   if line.startswith('data:'):
    results[port]=f'puerto={port} instancia={instance} {line}'
    break
  response.close()
 except Exception as ex: results[port]=f'error={type(ex).__name__}'
threads=[threading.Thread(target=listen,args=(p,i),daemon=True) for i,p in enumerate((5122,5123))]
for t in threads:t.start()
if not all(e.wait(12) for e in ready):raise RuntimeError('Los dos clientes no conectaron')
subprocess.run(['kubectl','-n','techrent-lab','exec','deploy/postgres','--','psql','-U','postgres','-d','TechRentDB','-c',"SELECT pg_notify('techrent_reservas','1501');"],check=True,capture_output=True)
for t in threads:t.join(15)
output='\n'.join(results.get(p,f'puerto={p} sin evento') for p in (5122,5123))
Path('_audit_work/eventos-replicas.txt').write_text(output,encoding='utf-8')
print(output)
