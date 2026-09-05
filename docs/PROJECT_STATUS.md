# Estado del Proyecto

Leer este archivo antes de realizar cambios.

Documentos relacionados: [PROJECT_SCOPE.md](PROJECT_SCOPE.md), [ARCHITECTURE.md](ARCHITECTURE.md) y [DECISIONS.md](DECISIONS.md).

## Repositorio

- Rama: `main`
- Remote: `https://github.com/Will-M7/Impresiones.git`
- Ultimo commit publicado y aprobado: `b8448fd9d89f75e5a93c7dbd359b04ea37e87e50`
- Mensaje: `feat: add apply settings to all documents`

## Estado del Commit 02

- Estado: `COMPLETADO, AUDITADO Y APROBADO`
- Commit previsto: `docs: add approved scope and architecture decisions`
- Hash del Commit 02: `cc9ec1e21799e422ba81e0067ab8ea7115b990f7`
- Auditoria documental de OpenCode: aprobada.
- Build: correcto.
- Warnings: 0.
- Tests detectados: 3.
- Tests aprobados: 3.
- Tests fallidos: 0.
- Revision manual del usuario: aprobada el 2026-08-31.
- Alcance funcional: aprobado.
- Arquitectura: aprobada.
- Decisiones: aprobadas.
- Estado del proyecto: aprobado.
- Bloqueadores del Commit 02: ninguno.
- Siguiente commit autorizado despues de publicar: `Commit 03 — feat: add application configuration and data paths`

El Commit 02 fue publicado y queda como base aprobada para el Commit 03.

## Estado del Commit 03

- Estado: `COMPLETADO, AUDITADO Y APROBADO`
- Commit previsto: `feat: add application configuration and data paths`
- Hash del Commit 03: `0f2a2fd6d74a3eddb1293c02f38d2831a80b86d4`
- Objetivo: centralizar la configuracion de rutas de datos, resolverlas de forma segura bajo una raiz autorizada y crear las carpetas requeridas de manera idempotente.
- Pruebas automaticas agregadas: resolucion de rutas, proteccion contra escape, carga JSON y creacion idempotente de directorios.
- Total de pruebas del Commit 03: 21.
- Restore: correcto.
- Build: correcto.
- Errores: 0.
- Warnings: 0.
- Tests detectados: 21.
- Tests aprobados: 21.
- Tests fallidos: 0.
- Tests omitidos: 0.
- Auditoria tecnica inicial de OpenCode: realizada.
- Hallazgo menor de normalizacion: corregido.
- Reauditoria dirigida de OpenCode: aprobada.
- Prueba manual del usuario: aprobada el 2026-08-31.
- Creacion exacta de las ocho carpetas: comprobada.
- Inicializacion idempotente: comprobada.
- Rechazo de ruta con `..`: comprobado.
- Mensaje controlado ante configuracion invalida: comprobado.
- Ausencia de creacion parcial ante configuracion invalida: comprobada.
- Restauracion de la configuracion de salida: comprobada.
- Bloqueadores actuales: ninguno.
- Siguiente commit despues de publicar el Commit 03: `Commit 04 — feat: add print job domain model`

## Estado del Commit 04

- Estado: `COMPLETADO, AUDITADO Y APROBADO`
- Commit previsto: `feat: add print job domain model`
- Hash del Commit 04: `550564e90cb7d120391016c1b72b7c41d43c68b4`
- Objetivo: agregar el modelo puro de dominio para trabajos de impresion, documentos imprimibles, configuracion individual, estados y transiciones validas.
- Modelo agregado: `PhoneNumber`, `PrintSettings`, `PrintDocument`, `PrintJob`, enums de dominio y `DomainRuleException`.
- Pruebas automaticas agregadas: validacion de telefono, configuracion de impresion, estados de documento, transiciones, encapsulacion del trabajo y aplicacion de configuracion a documentos editables.
- Restore: correcto.
- Build: correcto.
- Errores: 0.
- Warnings: 0.
- Tests detectados: 92.
- Tests aprobados: 92.
- Tests fallidos: 0.
- Tests omitidos: 0.
- Auditoria inicial de OpenCode: `COMMIT 04 REQUIERE CORRECCIÓN`.
- Hallazgos IMPORTANTES de cobertura: faltaba prueba explicita para rechazo de nueve digitos Unicode no ASCII y faltaba prueba explicita de coherencia de hash para valores iguales de `PhoneNumber`.
- Hallazgo MENOR de cobertura: la desigualdad de `PrintSettings` no cubria individualmente todas sus propiedades.
- Correccion de hallazgos: los tres puntos fueron cubiertos mediante pruebas unitarias.
- Implementacion productiva de Domain: no necesito cambios.
- Reauditoria dirigida de OpenCode: `CORRECCIÓN APROBADA — LISTO PARA CIERRE`.
- Prueba manual: no requerida para este commit por tratarse de logica pura de Domain.
- Modelo de dominio validado: si.
- Integracion externa: no implementada.
- Acceso a archivos: no implementado.
- Bloqueadores actuales: ninguno.
- Commit 04 publicado: `550564e90cb7d120391016c1b72b7c41d43c68b4`
- Siguiente commit despues de aprobar el Commit 04: `Commit 05 — feat: add document print settings`

## Estado del Commit 05

- Estado: `COMPLETADO, AUDITADO Y APROBADO`
- Commit previsto: `feat: add document print settings`
- Objetivo: completar la configuracion predeterminada explicita de documentos y la copia independiente de `PrintSettings`.
- Nota de alcance: el Commit 04 adelanto `PrintSettings`, sus enums y su asociacion con `PrintDocument`; el Commit 05 completa defaults y copia independiente sin duplicar codigo.
- `PrintSettings.Default`: A4, blanco y negro, una cara, vertical y una copia.
- `PrintSettings.Copy()`: copia independiente.
- Compatibilidad de `PrintDocument`: se agrego creacion sin configuracion explicita usando `PrintSettings.Default`.
- Restore: correcto.
- Build: correcto.
- Errores: 0.
- Warnings: 0.
- Tests detectados: 99.
- Tests aprobados: 99.
- Tests fallidos: 0.
- Tests omitidos: 0.
- Auditoria de OpenCode: `COMMIT 05 APROBADO PARA PUBLICACIÓN`.
- Pruebas automaticas agregadas: defaults explicitos, igualdad por valor, referencias independientes, copia independiente, variante via `with` y creacion de documentos con defaults.
- Prueba manual: no requerida para este commit por tratarse de logica pura de Domain.
- Hallazgo 07 conservado: el rechazo de audio, video y formatos no permitidos se atendera en el Commit 07.
- Hallazgo 09 conservado: la normalizacion y agrupacion por telefono se atendera en el Commit 09.
- Metadata Git: se elimino un `REBASE_HEAD` residual confirmado como metadata obsoleta, sin impacto sobre commits, staging o working tree.
- Bloqueadores actuales: ninguno.
- Commit 05 publicado: `9bb0354fd90e2c70ea6216fa19c09d565cfe1607`
- Siguiente commit despues de auditar y publicar el Commit 05: `Commit 06 — feat: add apply settings to all documents`

## Estado del Commit 06

- Estado: `COMPLETADO, AUDITADO Y APROBADO`
- Commit previsto: `feat: add apply settings to all documents`
- Objetivo: implementar en Application el caso de uso que aplica la configuracion del documento origen a todos los documentos editables del mismo trabajo.
- Auditoria de OpenCode: `COMMIT 06 APROBADO PARA PUBLICACIÓN`.
- Contratos agregados: comando, resultado, handler de aplicacion e interfaz `IPrintJobRepository`.
- Repositorio: abstraccion asincrona con `CancellationToken` para obtener y guardar `PrintJob`.
- Reutilizacion de Domain: el caso de uso usa `PrintJob.ApplySettingsToEditableDocuments`.
- Ajuste minimo de Domain: cada documento editable recibe una copia independiente mediante `PrintSettings.Copy()`.
- Documentos terminales: `Printed` y `Discriminated` permanecen intactos.
- Guardado unico mediante `IPrintJobRepository`.
- Estados de documentos: no se modifican durante la operacion.
- Persistencia real: no implementada.
- Pruebas automaticas agregadas: aplicacion a `Pending` y `Processing`, omision de terminales, copias independientes, edicion individual posterior, guardado unico, conteos, errores controlados, validacion de identificadores, propagacion de `CancellationToken`, trabajo sin editables y conservacion de estados.
- Restore: correcto.
- Build: correcto.
- Errores: 0.
- Warnings: 0.
- Tests detectados: 118.
- Tests aprobados: 118.
- Tests fallidos: 0.
- Tests omitidos: 0.
- Prueba manual: no requerida por tratarse de logica de Domain/Application.
- Hallazgo 07 conservado: el rechazo de audio, video y formatos no permitidos se atendera en el Commit 07.
- Hallazgo 09 conservado: la normalizacion y agrupacion por telefono se atendera en el Commit 09.
- Bloqueadores actuales: ninguno.
- Commit 06 publicado: `b8448fd9d89f75e5a93c7dbd359b04ea37e87e50`
- Siguiente commit despues de auditar y publicar el Commit 06: `Commit 07 — feat: add printable file validation`

## Estado del Commit 07

- Estado: `COMPLETADO, AUDITADO Y APROBADO PARA PUBLICACIÓN`
- Commit previsto: `feat: add printable file validation`
- Objetivo: validar archivos imprimibles por extension antes de admitirlos en la aplicacion o descargarlos desde Baileys.
- Auditoria inicial de OpenCode: `COMMIT 07 REQUIERE CORRECCIÓN`.
- Reauditoria final de OpenCode: `REQUIERE NUEVA CORRECCIÓN`.
- Hallazgo bloqueante 1: el nombre recibido desde WhatsApp podia conservar componentes de ruta y habilitar path traversal al construir el destino.
- Correccion 1 aplicada: Baileys obtiene un nombre base seguro, elimina componentes de ruta `/` y `\`, rechaza nombres invalidos despues del saneamiento y mantiene pruebas para rutas absolutas, segmentos `..` y separadores.
- Hallazgo de seguridad 2: la prueba manual revelo que el filtro `messages.upsert` sin `type === "notify"` provoca descarga de historial completo.
- Hallazgo de seguridad 3: los mensajes de libsignal en consola exponen estructuras criptograficas.
- Hallazgo 4: ausencia de deduplicacion provoca duplicados bajo reconexion.
- Correcciones 2-4 construidas: politica de eventos estricta (`notify`), deduplicacion persistente con SHA-256 y filtro seguro del logger.
- Hallazgo H1: el limite de persistencia era correcto, pero el conjunto en memoria podia superar 10000 entradas.
- Correccion H1 construida: el almacen de mensajes procesados limita simultaneamente persistencia y memoria a las 10000 claves mas recientes, expulsa antiguas de ambos estados y evita duplicados.
- Hallazgo H2: el test de ruta calculaba incorrectamente `D:\data\Temp`.
- Correccion H2 construida: las pruebas de ruta resuelven el directorio esperado desde el archivo de prueba y comparan rutas completas dentro de `data\Temp`.
- Hallazgo preventivo: la cobertura literal del filtro seguro no demostraba los mensajes reales de libsignal y usaba patrones demasiado genericos.
- Correccion preventiva construida: el filtro reconoce prefijos concretos de libsignal de forma case-insensitive y bloquea la llamada completa sin reenviar objetos asociados.
- Persistencia segura construida: `markCompleted` persiste primero el nuevo estado limitado y solo despues confirma memoria; ante fallo conserva el estado previo, libera `inProcessing`, limpia temporal y permite reintento.
- Ajuste de alcance: la validacion MIME completa fue retirada para coincidir con el Plan Maestro; el contrato futuro de MIME se conserva sin usar el MIME declarado como prueba de validez.
- Validacion .NET: contratos en Application e implementacion en Infrastructure para nombre de archivo, extension, tipo imprimible resultante y motivo controlado de rechazo.
- Formatos permitidos: PDF, Word, PowerPoint e imagenes JPG, JPEG, PNG, WEBP y BMP.
- Formatos rechazados: Excel, CSV, TXT, BIN, ejecutables, audio, video, MP4 y extensiones no permitidas por lista blanca de extension.
- Baileys: MP4, video, audio y stickers se rechazan antes de descargar contenido.
- Chats directos y grupos aceptados; broadcasts, newsletters y canales excluidos.
- En grupos, el remitente se obtiene de `message.key.participant`; el identificador `@g.us` nunca se usa como numero.
- Flujo heredado de audio: retirado o desactivado junto con referencias a rutas antiguas sin uso.
- Alcance tecnico: no se inspeccionan firmas binarias ni contenido interno; no se mueven, copian ni eliminan archivos desde el validador.
- Pruebas automaticas agregadas o ajustadas: politica de eventos, deduplicacion persistente limitada, persistencia recuperable, logger seguro literal y pruebas Node existentes de validacion de formatos.
- Pruebas tecnicas finales: Node 59/59 y .NET 154/154 aprobadas; build con 0 warnings y 0 errores.
- Reauditoria tecnica final: aprobada.
- Prueba manual final: aprobada.
- Sesion anterior revocada y credenciales locales retiradas.
- Nueva sesion vinculada correctamente mediante QR.
- Imagen JPG de chat directo guardada en `data\Inbox`.
- PDF de chat directo guardado.
- XLSX rechazado por extension.
- MP4 enviado como video rechazado por tipo.
- MP4 enviado como documento rechazado por extension.
- Audio o nota de voz rechazado por tipo.
- PDF enviado desde un grupo guardado correctamente.
- En grupos se utilizo el participante como remitente, nunca el JID `@g.us`.
- WhatsApp no recibio respuestas automaticas.
- Tras reiniciar Baileys sin enviar mensajes se crearon 0 archivos.
- No reaparecieron estructuras criptograficas, claves ni buffers en consola.
- El almacen persistente que inicialmente estaba dañado se reinicializo de forma segura y el siguiente reinicio ya no presento la advertencia.
- Se observo un identificador LID de quince digitos en el nombre de los archivos.
- La resolucion LID a telefono real continua expresamente reservada al Commit 09.
- Commit 07: prueba tecnica y prueba manual aprobadas.
- Hallazgo 07: cerrado en este commit.
- Hallazgo 09 conservado: la normalizacion y agrupacion por telefono, incluidos identificadores de 15 digitos, continua pendiente para el Commit 09.
- Bloqueadores actuales: ninguno.
- Siguiente commit despues de auditar y publicar el Commit 07: `Commit 08 — feat: add inbox monitoring service`

## Validaciones del Commit 01

- Restore correcto.
- Build correcto.
- Cero warnings.
- Tres tests aprobados.
- WPF abrio correctamente.
- Baileys conservo la conexion.
- Descarga general confirmada en `data/Inbox`.
- Auditoria de seguridad aprobada.
- Commit 01 publicado mediante fast-forward.
- Local y remoto sincronizados.

## Hallazgos Conocidos

- Baileys descargo archivos MP4 durante la prueba; corregido en preparacion del Commit 07 mediante rechazo antes de descarga.
- Existe comportamiento heredado relacionado con audio; retirado o desactivado en preparacion del Commit 07.
- Permanecen rutas heredadas de audio, Ordenes y ejecutados; referencias sin uso retiradas en preparacion del Commit 07.
- Algunos archivos utilizaron identificadores de 15 digitos.
- Hallazgo 07: el rechazo de audio, video y formatos no permitidos se atendera en el Commit 07.
- Hallazgo 09: la normalizacion y agrupacion por telefono se atendera en el Commit 09.

No se registran numeros telefonicos reales, nombres de clientes ni nombres de archivos reales en este documento.

## Siguiente Secuencia

- Commit actual aprobado para publicacion: Commit 07.
- Siguiente paso: `Commit 08 — feat: add inbox monitoring service`.
- No iniciar el Commit 08 hasta cerrar y publicar el Commit 07.

## Bloqueadores

Ninguno para preparar el Commit 07.

## Continuidad

Despues de cada commit debe actualizarse:

- Estado.
- Commit aprobado.
- Pruebas ejecutadas.
- Resultados manuales.
- Hallazgos.
- Bloqueadores.
- Siguiente commit autorizado.
