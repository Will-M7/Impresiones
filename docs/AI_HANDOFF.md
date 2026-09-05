# Ficha Activa de IA

## Commit Objetivo

Commit 08 - `feat: add inbox monitoring service`

No iniciar este commit hasta recibir autorizacion explicita del usuario. Esta ficha solo prepara el contexto compartido.

## Base Aprobada

- Ultimo commit funcional aprobado: Commit 07.
- Hash: `f0c9f26b7178b25897fd529da10c04ac69f9a7c3`
- Mensaje: `feat: add printable file validation`

## Alcance Autorizado

- Crear `IInboxMonitor` en Application.
- Crear implementacion basada en `FileSystemWatcher` en Infrastructure.
- Observar eventos `Created` y `Renamed`.
- Realizar reescaneo inicial de archivos preexistentes.
- Aplicar debounce y deduplicacion de notificaciones.
- Comprobar estabilidad de tamano y posibilidad de apertura.
- Publicar rutas normalizadas.
- Soportar cancelacion y liberacion limpia de recursos.
- Recuperarse de errores del watcher sin quedar detenido.

## Fuera de Alcance

- No validar extensiones nuevamente.
- No agrupar por telefono.
- No mover archivos.
- No crear `PrintJob` ni `PrintDocument`.
- No integrar todavia la interfaz WPF.
- No persistir en SQLite.
- No iniciar el Commit 09.

## Decisiones Relevantes

- Clean Architecture sigue vigente.
- Application define contratos; Infrastructure implementa adaptadores del sistema operativo.
- Baileys sigue siendo proceso Node.js separado y deposita archivos en `data/Inbox`.
- La validacion de formatos ya pertenece al Commit 07 y no debe duplicarse aqui.
- Codex escribe; OpenCode audita sin modificar; Antigravity puede explorar, planificar y verificar.
- Solo un escritor puede operar en el mismo worktree.

## Validaciones Esperadas por Responsable

### Codex

- Ejecutar `git status --short` antes y despues.
- Ejecutar solamente las pruebas focalizadas de `IInboxMonitor` y `FileSystemInboxMonitor` que resulten necesarias durante la implementacion.
- No ejecutar obligatoriamente `dotnet build --configuration Debug`, `dotnet test --configuration Debug` ni toda la suite de Baileys durante la implementacion.
- Ejecutar `git diff --check`.
- Revisar el diff para confirmar que no se adelanto Commit 09.

### OpenCode

- Ejecutar `dotnet build --configuration Debug`.
- Ejecutar `dotnet test --configuration Debug`.
- Ejecutar las suites completas adicionales que exija el alcance o la auditoria.
- Revisar diff, regresiones, secretos y archivos runtime.
- Usar como linea base actual antes del Commit 08: .NET completo esperado, 154 pruebas; Baileys completo esperado, 59 pruebas.

## Entrega Esperada

- Archivos modificados.
- Resumen de implementacion.
- Resultado de build y pruebas.
- Riesgos o dudas.
- Estado Git final.
