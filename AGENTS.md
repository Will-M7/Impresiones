# AGENTS

Este repositorio usa este archivo como instruccion comun para Codex, OpenCode y Antigravity.

## Fuentes de Verdad

- `docs/PROJECT_STATUS.md`: estado operativo y siguiente commit.
- `docs/PROJECT_SCOPE.md`: alcance funcional aprobado.
- `docs/ARCHITECTURE.md`: arquitectura y dependencias permitidas.
- `docs/DECISIONS.md`: decisiones consolidadas.
- `docs/AI_HANDOFF.md`: ficha activa del commit en preparacion.
- `.agents/skills`: procedimientos bajo demanda.

## Reglas

- La solucion sigue Clean Architecture: Domain no depende de otras capas, Application depende de Domain, Infrastructure depende de Application y Domain, Desktop actua como Composition Root.
- Baileys permanece como proceso Node.js separado y se comunica por `data/Inbox`.
- No ampliar el alcance del commit activo ni adelantar commits posteriores.
- Codex es el unico escritor principal.
- OpenCode audita sin modificar archivos.
- Antigravity puede explorar, planificar y verificar sin asumir escritura principal.
- Antigravity no participa obligatoriamente en cada commit; se utiliza selectivamente para exploracion, planificacion, investigacion o diagnostico complejo.
- No debe haber escritores simultaneos en el mismo worktree.
- No crear commit ni hacer push sin autorizacion explicita del usuario.
- No versionar credenciales, sesiones, runtime local, secretos ni datos operativos reales.

## Verificacion Minima

Antes de declarar terminado, revisar `git status --short`, ejecutar las pruebas pertinentes, revisar el diff y confirmar que solo cambiaron archivos dentro del alcance autorizado.

## Carga Bajo Demanda

Leer documentacion y skills solo cuando sean necesarias para el trabajo activo. Evitar copiar instrucciones extensas entre herramientas; intercambiar resumen, alcance, comandos ejecutados, hallazgos y estado Git.
