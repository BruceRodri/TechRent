import re,urllib.request,urllib.parse,http.cookiejar
from pathlib import Path
jar=http.cookiejar.CookieJar(); browser=urllib.request.build_opener(urllib.request.HTTPCookieProcessor(jar)); base='http://127.0.0.1:5124'
def anti(body):return re.search(r'name="__RequestVerificationToken"[^>]*value="([^"]+)',body).group(1)
login=browser.open(base+'/Identity/Account/Login').read().decode()
browser.open(base+'/Identity/Account/Login',data=urllib.parse.urlencode({'Input.Email':'admin@techrent.com','Input.Password':'Admin@123','__RequestVerificationToken':anti(login)}).encode()).read()
get=browser.open(base+'/Equipos/Create'); page=get.read().decode()
assert get.status==200 and 'Nombre' in page
result=browser.open(base+'/Equipos/Create',data=urllib.parse.urlencode({'__RequestVerificationToken':anti(page)}).encode()); html=result.read().decode()
assert result.status==200 and 'El nombre del equipo es obligatorio' in html
out='GET /Equipos/Create: HTTP 200\nPOST /Equipos/Create con formulario vacío: HTTP 200, validación de nombre obligatorio mostrada\nNo se creó ningún equipo.'
Path('_audit_work/formulario-final.txt').write_text(out,encoding='utf-8');print(out)
