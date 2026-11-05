import re
import urllib.request, urllib.parse, http.cookiejar
from pathlib import Path

jar=http.cookiejar.CookieJar()
session=urllib.request.build_opener(urllib.request.HTTPCookieProcessor(jar))
base='http://127.0.0.1:5122'
login=session.open(base+'/Identity/Account/Login',timeout=10).read().decode()
token=re.search(r'name="__RequestVerificationToken"[^>]*value="([^"]+)',login).group(1)
data=urllib.parse.urlencode({'Input.Email':'admin@techrent.com','Input.Password':'Admin@123','__RequestVerificationToken':token}).encode()
session.open(base+'/Identity/Account/Login',data=data,timeout=10).read()
lines=[]
for port in (5122,5123):
    result=session.open(f'http://127.0.0.1:{port}/Reservas',timeout=10)
    lines.append(f'puerto={port} status={result.status} url={result.url} instancia={result.headers.get("X-TechRent-Instance", "no-informada")}')
Path('_audit_work/sesion-replicas.txt').write_text('\n'.join(lines),encoding='utf-8')
print('\n'.join(lines))
