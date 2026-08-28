# Impresiones

Impresiones es una aplicacion local de escritorio Windows para gestionar e imprimir documentos recibidos mediante WhatsApp de forma rapida, sencilla y accesible para operadores no tecnicos.

## Stack

- .NET 10
- C#
- WPF
- XAML
- Clean Architecture
- SQLite (futuro)
- Node.js + Baileys

## Estado actual

Esta inicializacion contiene la estructura base de la solucion, proyectos por capa y una ventana WPF minima para validar el arranque.

Baileys funciona exclusivamente como adaptador de recepcion de WhatsApp: valida mensajes, descarga archivos soportados, los renombra y guarda las descargas generales en `data/Inbox`.

No se incluyen todavia impresion real, previews, SQLite, Office Interop, precios, integracion entre Baileys y C#, instalador ni diseno definitivo.
