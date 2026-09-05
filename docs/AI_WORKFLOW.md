# Flujo de Colaboracion de IA

Este flujo coordina Codex, OpenCode y Antigravity sin duplicar instrucciones. `AGENTS.md` es la instruccion comun; `docs/AI_HANDOFF.md` es la ficha activa del siguiente commit.

## 1. Preparar la Ficha Activa

- Actualizar `docs/AI_HANDOFF.md` con commit objetivo, alcance autorizado, fuera de alcance, decisiones relevantes y validaciones esperadas.
- Confirmar que `docs/PROJECT_STATUS.md` identifica el ultimo commit funcional aprobado y el siguiente commit funcional.
- No incluir secretos, rutas de sesiones, credenciales ni datos reales.

## 2. Implementacion por Codex

- Codex actua como implementador principal y unico escritor del worktree.
- Debe leer `AGENTS.md`, `docs/AI_HANDOFF.md` y los documentos necesarios bajo demanda.
- Debe verificar Git antes de escribir, explorar solo archivos pertinentes e implementar dentro del alcance.
- Ejecutar solamente las pruebas focalizadas necesarias para obtener retroalimentacion del componente modificado. La compilacion y las suites completas corresponden a la auditoria independiente de OpenCode.
- Debe revisar el diff antes de entregar.
- No debe crear commit, hacer push ni preparar staging sin autorizacion expresa.

## 3. Auditoria por OpenCode

- OpenCode trabaja sin modificar archivos.
- Debe revisar el diff exacto contra la base aprobada o contra el estado indicado por Codex.
- Debe ejecutar build y suites completas, comprobar regresiones, alcance, decisiones, arquitectura, pruebas y riesgos.
- Debe clasificar hallazgos como H1, H2 o H3:
  - H1: bloqueante por seguridad, perdida de datos, ruptura de build, pruebas criticas fallidas o incumplimiento claro de alcance.
  - H2: correccion necesaria antes de aprobar por bug probable, cobertura insuficiente importante o desviacion de arquitectura.
  - H3: mejora menor, claridad, mantenimiento o cobertura preventiva.
- Solo aprueba cuando no quedan H1 ni H2.

## 4. Uso Opcional de Antigravity

- Antigravity puede explorar, planificar, verificar comportamiento o revisar riesgos.
- No reemplaza a Codex como escritor principal.
- No debe escribir simultaneamente sobre el mismo worktree.
- Sus conclusiones deben volver como resumen accionable, no como transcripcion extensa.

## 5. Correccion de H1/H2

- Codex corrige H1 y H2 dentro del mismo alcance autorizado.
- Cada correccion debe ser minima y verificable.
- Luego se repiten pruebas pertinentes y reauditoria dirigida.
- H3 puede diferirse si no bloquea el cierre y queda registrado cuando sea util.

## 6. Prueba Manual del Usuario

- El usuario realiza validaciones manuales cuando el commit toca integraciones, escritorio, WhatsApp, archivos reales o flujos visibles.
- El usuario autoriza commit y push cuando corresponda.
- Codex debe entregar pasos concretos y registrar resultado, fecha y observaciones relevantes en `docs/PROJECT_STATUS.md` cuando el usuario lo apruebe.

## 7. Autorizacion de Commit y Push

- Commit y push requieren autorizacion explicita.
- Antes de commit: `git status --short`, pruebas requeridas, `git diff --check` y confirmacion de que no hay archivos fuera de alcance.
- No usar `git add .`; preparar solo archivos autorizados.

## 8. Cierre y Preparacion del Siguiente Commit

- Al cerrar un commit, actualizar estado, hash aprobado, pruebas ejecutadas, hallazgos, bloqueadores y siguiente commit autorizado.
- La ficha `docs/AI_HANDOFF.md` debe prepararse para el siguiente commit funcional despues de cerrar el actual.
- No iniciar implementacion del siguiente commit hasta que el usuario lo autorice.

## Informacion a Intercambiar

- Hash base y estado Git.
- Objetivo del commit activo.
- Alcance autorizado y fuera de alcance.
- Archivos modificados.
- Comandos ejecutados y resultado resumido.
- Diff relevante o rutas con lineas cuando haya hallazgos.
- Riesgo, impacto y correccion minima para H1/H2/H3.

## Salidas que No Deben Copiarse

- Logs completos de build o pruebas si bastan resultados resumidos.
- Diffs extensos sin hallazgos concretos.
- Transcripciones completas entre herramientas.
- Datos reales de clientes, telefonos, nombres de archivos, credenciales, tokens o sesiones.
- Contenido de `data/`, runtime local o carpetas de autenticacion.
