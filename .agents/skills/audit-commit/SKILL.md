---
name: audit-commit
description: Audita una implementación en modo de solo lectura. Usar cuando OpenCode revise el diff, ejecute la validación completa, clasifique hallazgos H1/H2/H3 y no modifique archivos.
---
# Audit Commit

Usar esta skill cuando OpenCode u otra herramienta audite un commit.

1. Trabajar sin modificar archivos.
2. Leer `AGENTS.md`, `docs/AI_HANDOFF.md`, `docs/DECISIONS.md` y el estado del proyecto.
3. Revisar el diff exacto del commit o del worktree indicado.
4. Comprobar alcance, arquitectura, decisiones y dependencias permitidas.
5. Ejecutar pruebas pertinentes cuando sea posible.
6. Clasificar hallazgos como H1, H2 o H3 usando las definiciones oficiales de `docs/AI_WORKFLOW.md`.
7. Para cada hallazgo incluir evidencia, impacto y correccion minima.
8. Aprobar solo cuando no queden H1 ni H2.

No crear commit, no hacer push, no preparar staging y no reescribir la implementacion.
