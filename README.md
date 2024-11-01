# UCR.WEB.Blog
## Autores 
- Isaac Gutiérrez Ramírez - C03640
- Christopher Durán Aguilar - B72630
## Base de datos 
Trabajamos una base de datos sql server, la estamos corriendo en docker para facilitar el uso de esta. 
Pasos para correr la base de datos:
- En la carpeta raíz se encuentra el archivo sql_server_container_image.tar. En el encontramos una imagen del contenedor de docker con la base de datos
- Ocupamos tener docker operativo en el sistema, para ello lo puede instalar en la pagina oficial https://www.docker.com/. La instalación varia dependiendo del SO.
- Teniendo docker en el sistema vamos a crear un contenedor con la imagen brindada sql_server_container_image.tar. Esta imagen no está en el repositorio debido a su tamaño, se debería de agregar por aparte. Si no cuenta con este archivo porfavor solicitarlo a uno de los autores.
- Para crear el contenedor a partir de esta imagen corremos el siguiente comando 
 ```bash
 docker load -i sql_server_container_image.tar
```
- Una vez cargado el contenedor podemos ponerlo a ejecutar con el siguiente comando 
 ```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MYPassword" -p 1433:1433 --name sql_server_container -v sqlserver_data:/var/opt/mssql -d sql_server_container_image
```
- El comando anterior puede dar problemas si el volumen no está guardado en la dirección /var/opt/mssql, para solucionarlo debe de encontrar donde docker guarda los volumenes en su computadora
- Estamos al tanto de nuestra brecha de seguridad al tener la contraseña quemada en el archivo de configuración. No creemos necesario validar ese paso para esta tarea
- Puede comprobar que su contenedor este corriendo con el comando
 ```bash
 docker ps
```
- Si gusta puede usar un adminstrador de bases de datos para ver la base de datos en el puerto 1433. Si no, este programa se va a encargar de hacer lo necesario para lo requerido en el proyecto.



