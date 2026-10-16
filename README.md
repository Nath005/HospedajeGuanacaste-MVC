# HospedajeGuanacaste

Sistema web para la gestión de reservaciones, clientes, empleados y habitaciones de un emprendimiento de hospedaje. Desarrollado en C# (.NET 10) con arquitectura MVC, API REST y base de datos SQL Server.

### Características
- Arquitectura multicapa con separación de responsabilidades.
- Capa web en ASP.NET Core MVC para la interfaz de usuario.
- Web API RESTful para el procesamiento de solicitudes y reglas de negocio.
- Acceso a datos independiente mediante ADO.NET.
- Base de datos relacional en SQL Server.
- Módulos completos de Clientes, Empleados, Habitaciones y Reservaciones (CRUD y filtros de búsqueda).
- Generación de reportes de reservaciones semanales ordenados por monto.

### Estructura del Repositorio
- `HospedajeGuanacaste/`: Proyecto web (Vistas Razor y Controladores).
- `HospedajeGuanacaste.API/`: API RESTful que procesa la lógica de negocio.
- `HospedajeGuanacaste.Datos/`: Capa de datos y modelos del sistema.
- `BaseDeDatos/`: Script SQL para la creación de la base de datos y esquemas.
- `HospedajeGuanacaste.slnx`: Archivo de solución para Visual Studio.

### Ejecución del Proyecto
1. **Base de Datos:** Ejecuta el script SQL ubicado en la carpeta `BaseDeDatos/` en tu servidor SQL Server.
2. **Conexión:** Verifica la cadena de conexión en `HospedajeGuanacaste.Datos/ConexionBD.cs` según tu instancia local.
3. **Solución:** Abre el archivo `HospedajeGuanacaste.slnx` en Visual Studio.
4. **Inicio Múltiple:** Configura la solución para iniciar al mismo tiempo los proyectos `HospedajeGuanacaste.API` y `HospedajeGuanacaste` (MVC), luego presiona `F5`.

<img width="921" height="414" alt="image" src="https://github.com/user-attachments/assets/72c7eef0-4157-41c0-8bae-30a6a7b81071" />

<img width="864" height="404" alt="image" src="https://github.com/user-attachments/assets/bceb8395-ebee-4056-b64c-1a40eb023cb0" />

<img width="905" height="396" alt="image" src="https://github.com/user-attachments/assets/0a14a9ac-c173-45a8-b77e-9965fc1058c1" />

<img width="691" height="412" alt="image" src="https://github.com/user-attachments/assets/44803c98-241d-451d-95fc-be0bbb9e37d2" />

<img width="654" height="402" alt="image" src="https://github.com/user-attachments/assets/68fe3516-039b-42ca-a4df-583799e3c78e" />

<img width="844" height="397" alt="image" src="https://github.com/user-attachments/assets/3af5c57f-0470-40b3-b384-9c9ae8660f57" />

<img width="836" height="409" alt="image" src="https://github.com/user-attachments/assets/e00795e7-8cd8-4c1b-8f0c-c3736699a22b" />

<img width="847" height="407" alt="image" src="https://github.com/user-attachments/assets/6e07febe-5da7-4de6-a260-084985225564" />







