---
name: implement-commit
description: Implementa un commit autorizado dentro del alcance definido en AI_HANDOFF. Usar cuando Codex construya o corrija código, ejecute pruebas focalizadas y entregue cambios sin commit ni push.
---
# Implement Commit

Usar esta skill cuando Codex implemente un commit autorizado.

1. Leer `AGENTS.md` y `docs/AI_HANDOFF.md`.
2. Verificar `git status --short` y confirmar la base indicada.
3. Explorar solo archivos pertinentes al alcance del commit.
4. Implementar cambios minimos dentro del alcance autorizado.
5. No modificar decisiones existentes salvo que el usuario lo pida.
6. Ejecutar solamente las pruebas focalizadas necesarias para obtener retroalimentacion del componente modificado; build y suites completas corresponden a OpenCode.
7. Ejecutar `git diff --check`.
8. Revisar el diff para detectar archivos fuera de alcance, secretos, sesiones, runtime local o dependencias no autorizadas.
9. Entregar resumen, resultados de validacion, riesgos y estado Git final.

No crear commit, no hacer push, no preparar staging y no publicar automaticamente.
