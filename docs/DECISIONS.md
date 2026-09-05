# Decisiones

Decisiones consolidadas el 2026-08-30. Si la fecha original de una decision no se conoce, se registra como consolidada en la fecha de este documento.

Ver tambien [PROJECT_SCOPE.md](PROJECT_SCOPE.md), [ARCHITECTURE.md](ARCHITECTURE.md) y [PROJECT_STATUS.md](PROJECT_STATUS.md).

## Decisiones Aprobadas

1. 2026-08-30: La V1 sera exclusivamente una aplicacion de escritorio.
2. 2026-08-30: Los sistemas objetivo son Windows 10 x64 y Windows 11 x64.
3. 2026-08-30: El stack principal sera C#, .NET 10, WPF, XAML y MVVM.
4. 2026-08-30: La solucion seguira Clean Architecture con dependencias apuntando hacia Domain.
5. 2026-08-30: Baileys permanecera como proceso Node.js separado.
6. 2026-08-30: La comunicacion entre Baileys y la aplicacion se realizara mediante `data/Inbox`.
7. 2026-08-30: SQLite queda reservado para Infrastructure y trabajo posterior.
8. 2026-08-30: La integracion de impresoras usara Windows Printing APIs posteriormente.
9. 2026-08-30: Word y PowerPoint se integraran mediante Office Interop posteriormente.
10. 2026-08-30: Las impresoras se detectaran sin hardcodear modelos.
11. 2026-08-30: El operador trabajara con un numero telefonico a la vez.
12. 2026-08-30: Cada documento tendra configuracion independiente.
13. 2026-08-30: "Aplicar a todos" sera una operacion de copia, no un vinculo permanente.
14. 2026-08-30: `Printed` y `Discriminated` seran destinos finales del flujo.
15. 2026-08-30: Publisher se implementara al final.
16. 2026-08-30: No se desarrollara movil en la V1.
17. 2026-08-30: Figma queda aprobado como referencia visual; su implementacion WPF sigue pendiente.
18. 2026-08-30: Credenciales y datos operativos nunca se versionan.
19. 2026-08-30: Los commits seran pequenos y usaran convenciones `chore:`, `feat:`, `fix:`, `refactor:`, `test:` y `docs:`.
20. 2026-08-30: Codex construye; OpenCode audita sin modificar; el usuario realiza las validaciones manuales necesarias.

21. 2026-09-03: Baileys acepta archivos imprimibles desde chats directos y grupos.
22. 2026-09-03: En grupos de WhatsApp, el propietario del mensaje es el participante remitente, no el identificador del grupo.
23. 2026-09-03: Estados, canales, newsletters y broadcasts quedan excluidos del procesamiento de Baileys.
24. 2026-09-03: La normalizacion de identificadores LID y de quince digitos continua pendiente para el Commit 09.
25. 2026-09-05: Antigravity queda aprobado como explorador, planificador y verificador; se mantiene la regla de un solo escritor por worktree.
26. 2026-09-05: `AGENTS.md` es la instruccion comun para Codex, OpenCode y Antigravity; `.agents/skills` contiene procedimientos bajo demanda.
27. 2026-09-05: No se introducen plugins ni MCP en este momento para el flujo de colaboracion de IA.

## Asuntos Pendientes

- Confirmar modelos exactos de impresoras.
- Confirmar capacidades reportadas por los drivers.
- Definir detalles finales de Publisher.
- Definir politica exacta de retencion y limpieza.
- Validar compatibilidad real con versiones instaladas de Microsoft Office.
