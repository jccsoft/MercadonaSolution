# MercadonaSolution
Obtención de productos y categorías de la API de Mercadona.

# Introducción 
El objetivo es conectar con la API de Mercadona y mostrar sus productos.

En la solución hay 5 proyectos:
1. Proyecto MercadonaAPI. 
    * Worker Service: Obtiene los productos y categorías directamente de la API de Mercadona (una vez al día, porque es muy costoso) y los guarda en ficheros json.
    * API: Responde a las peticiones de categorías y productos, desde los ficheros json obtenidos previamente.
2. Proyecto MercadonaAPI.BlazorServer:
    * Ejemplo de aplicación para ver los productos y categorías. 
    * Está hecha en Blazor Server y Mudblazor.
3. Proyecto MercadonaAPI.RazorPages:
    * Ejemplo de aplicación para ver los productos y categorías. 
    * Está hecha en Razor Pages, con dos versiones: reaprovechando los componentes del proyecto anterior, y la clásica con páginas Razor.
4. Proyecto MercadonaAPI.Shared:
    * Modelos en común a la API y los clientes.
5. Proyecto MercadonaAPI.Tests
    * Unit tests de la API.

# Web Apps
Las aplicaciones web están alojadas en Azure:
* MercadonaAPI: https://mrkdna.azurewebsites.net/swagger/index.html
* MercadonaAPI.BlazorServer: https://mrkdonablazor.azurewebsites.net
* MercadonaAPI.RazorPages: https://mrkdonarazor.azurewebsites.net

Dado que sólo son pruebas de concepto, el plan de Azure es el gratuito, por lo que puede tardar unos segundos en abrirse la primera vez.

# Configuración
* MercadonaAPI AppSettings.json:

Clave | Valor
------------ | -------------
BaseUrl | Url de la API de Mercadona 
FullDBFilePath | Dirección del fichero json completo obtenido de la API
CategoriesFilePath | Dirección del fichero json de categorías, obtenido del completo para acelerar las consultas
ProductsFilePath | Dirección del fichero json de productos, obtenido del completo para acelerar las consultas
ActiveWorkerService | True para indicar al Worker Service que renueve el fichero completo si lleva más de un día sin actualizarse. False, no se actualiza


* BlazorServer y Razorpages AppSettings.json:

Clave | Valor
------------ | -------------
AzureBaseUriString | Url de la API de Mercadona 
LocalBaseUriString | Url de la API local, si hemos publicado la API en nuestro IIS local.

En BuilderExtensions.cs podemos cambiar si usamos una Url o la otra (client.BaseAddress = new Uri(options.AzureBaseUriString))


* Tests AppSettings.json:

Clave | Valor
------------ | -------------
BaseUrl | Url de la API de pruebas. En mi caso, he subido un fichero (FullDBFile) a json.io 
FullDBFilePath | Dirección del fichero json completo obtenido de la API
CategoriesFilePath | Dirección del fichero json de categorías, obtenido del completo para acelerar las consultas
ProductsFilePath | Dirección del fichero json de productos, obtenido del completo para acelerar las consultas
X-Master-Key | <API_KEY> Required (del bin en json.io)
X-Access-Key | <ACCESS_KEY> Required
X-Bin-Meta | <true / false> No required
X-JSON-Path | <JSON_ACCESSOR> No required
