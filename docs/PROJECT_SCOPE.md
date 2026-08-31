# Alcance del Proyecto

Este documento define el alcance funcional aprobado de Impresiones. Debe leerse junto con [ARCHITECTURE.md](ARCHITECTURE.md), [DECISIONS.md](DECISIONS.md) y [PROJECT_STATUS.md](PROJECT_STATUS.md).

## Proposito

Impresiones es una aplicacion local de escritorio para Windows destinada a automatizar y simplificar un negocio de impresion. La experiencia debe poder ser usada por una persona con poca experiencia usando computadoras.

## Sistemas Objetivo

- Windows 10 x64.
- Windows 11 x64.
- Primera version exclusivamente de escritorio.
- No se desarrollara aplicacion movil en la V1.

## Stack Confirmado

- C#.
- .NET 10.
- WPF.
- XAML.
- MVVM.
- Clean Architecture.
- SQLite posteriormente.
- Windows Printing APIs posteriormente.
- Microsoft Office Interop posteriormente.
- Node.js 22 como entorno actual de Baileys.
- Baileys como componente separado.

Node.js 22 registra el entorno actual esperado para el componente de recepcion. No convierte la version instalada en una restriccion permanente sin validacion posterior.

## Responsabilidad de Baileys

Baileys sera responsable unicamente de:

1. Recibir archivos desde WhatsApp.
2. Validarlos.
3. Descargarlos.
4. Renombrarlos.
5. Guardarlos en `data/Inbox`.

Baileys no responde mensajes de WhatsApp, no contiene la logica de impresion y no contiene logica de negocio propia de WPF. Debe mantenerse pequeno, separado y fuera de la solucion .NET. La sesion y autenticacion de WhatsApp nunca se versionan.

## Formatos Imprimibles Permitidos

- PDF
- DOC
- DOCX
- DOCM
- PPT
- PPTX
- PPTM
- JPG
- JPEG
- PNG
- WEBP
- BMP

## Formatos Rechazados

- XLS
- XLSX
- XLSM
- CSV
- TXT
- Ejecutables
- BIN
- Audio
- Video

La validacion definitiva considerara extension y MIME o tipo real. Excel no forma parte de los formatos admitidos.

## Identificacion

La identificacion operativa usara los nueve digitos del telefono. Se ignora el prefijo internacional `+51`, no se necesita guardar el nombre del contacto y la normalizacion definitiva se implementara posteriormente.

Cuando se necesiten ejemplos, deben ser ficticios y no deben representar datos reales.

## Flujo de Archivos

```text
Baileys -> Inbox
Inbox -> Processing
Processing -> Printed
Processing -> Discriminated
```

Reglas aprobadas:

- Al comenzar el procesamiento, el archivo pasa a `Processing`.
- Al imprimirse correctamente, sale inmediatamente de Trabajos y pasa a `Printed`.
- Al descartarse, pasa a `Discriminated`.
- `Printed` y `Discriminated` no deben eliminarse mediante la limpieza automatica de pendientes.
- Los archivos pendientes antiguos podran eliminarse despues de aproximadamente siete dias mediante una politica futura y configurable.

En el Commit 02 no se implementa movimiento automatico entre carpetas.

## Trabajo Por Numero

El operador trabajara con los archivos de un numero a la vez. Cada documento mantiene configuracion independiente.

Configuraciones previstas:

- Tamano.
- Color o blanco y negro.
- Una cara o duplex.
- Demas opciones admitidas por la impresora y el flujo aprobado.

La accion "Aplicar esta configuracion a todos" copiara la configuracion actual a los demas documentos. Despues de copiar, cada documento podra editarse individualmente.

## Impresoras

La aplicacion detectara impresoras instaladas en Windows. Administracion permitira elegir una impresora Color y una impresora Blanco y negro.

No se deben hardcodear modelos. Los modelos Konica Minolta mencionados durante la planificacion no estan confirmados. Las capacidades reales deberan consultarse desde Windows o configurarse, sin inventarlas.

## Imagenes a Word

Flujo aprobado para trabajo posterior:

1. Seleccionar imagenes pertenecientes al mismo numero.
2. Configurar imagenes por hoja.
3. Elegir A4, orientacion y margenes estrechos.
4. Mantener relacion de aspecto.
5. Abrir Word.
6. Crear un documento editable.
7. Insertar las imagenes.
8. Permitir edicion libre en Word.
9. Mantener una ventana pequena del sistema con Color, Blanco y negro, Imprimir y Cancelar.

Este flujo todavia no esta implementado.

## Publisher

Publisher se utilizara posteriormente para una sola imagen. Dividira una impresion grande en varias hojas A3 y tendra configuracion de tamano, cantidad de hojas, orientacion y vista previa.

Publisher se implementara al final y no forma parte del trabajo actual.

## Exclusiones Actuales

- No movil.
- No frontend WPF definitivo en el Commit 02.
- No SQLite todavia.
- No impresion real todavia.
- No Office Interop todavia.
- No Publisher todavia.
- No previews todavia.
- No automatizacion funcional en este commit.
