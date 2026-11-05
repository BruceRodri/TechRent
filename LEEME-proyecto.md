# TechRent distribuido — laboratorio local

Esta carpeta adapta el proyecto base https://github.com/BruceRodri/TechRent. Los cambios están en `TechRent-distribuido/`, los manifiestos en `kubernetes/` y los scripts en `pruebas/`. El informe resume las pruebas del 4 de noviembre de 2026. La copia contiene datos y credenciales de demostración; no es una configuración de producción.

## Inicio en un clúster limpio de Docker Desktop

Desde la carpeta de este archivo, con Docker y Kubernetes activos:

```powershell
docker build -t techrent-distribuido:final ./TechRent-distribuido
docker build -t techrent-red:lab -f pruebas/net.Dockerfile pruebas
kubectl apply -f kubernetes/techrent.yaml
kubectl -n techrent-lab wait --for=condition=complete job/techrent-init --timeout=300s
kubectl -n techrent-lab rollout status deployment/techrent --timeout=120s
kubectl -n techrent-lab port-forward service/techrent 5121:80
```

Abrir `http://localhost:5121`. Las credenciales de demostración se encuentran en el código de inicialización del proyecto base. La carga inicial se limita a 1.000 equipos, 2.000 clientes, 1.500 reservas y 7.000 detalles. No usa los datos del despliegue anterior.

En otra consola se puede crear el ConfigMap y lanzar la prueba:

```powershell
kubectl -n techrent-lab create configmap k6-scripts --from-file=pruebas/k6-carga.js --from-file=pruebas/k6-resiliencia.js
kubectl apply -f kubernetes/pruebas.yaml
kubectl -n techrent-lab logs -f job/k6-carga
```

El laboratorio registrado usa un solo nodo, una base y conexiones HTTP locales. El reporte de ZAP es pasivo y no autenticado. Las claves Data Protection se comparten en un volumen local, pero la sesión general permanece en memoria; para varias máquinas hace falta almacenamiento compartido apropiado para esos componentes y protección de claves.
