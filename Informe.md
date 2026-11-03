# Archivos Físicos:

Para el proyecto simulamos discos duros externos en la carpeta C:\BDD-PYME . Los cuales son:
- C:\BDD-PYME\SSD(A) : Para BDD-PYME.mdf (Archivo maestro)
- C:\BDD-PYME\SSD(B) : Para BDD-PYME.ldf (Archivo de log)
- C:\BDD-PYME\RAID5(I) : Para TB01.ndf (Tablas con temperatura baja (6): PYMES, ALYCs, SGRs, BROKERS, INSTRUMENTOS, GARANTIAS)
- C:\BDD-PYME\RAID5(J) : Para TB02.ndf (Tablas con temperatura baja (6): PYMES, ALYCs, SGRs, BROKERS, INSTRUMENTOS, GARANTIAS)
- C:\BDD-PYME\RAID5(E) : Para TM01.ndf (Tablas con temperatura media (4): TAZA-DE-INTERES, GARANTIAS/ALYCs, GARANTIASDISPONIBLIDAD, BROKER/COMISIONES)
- C:\BDD-PYME\RAID5(F) : Para TM02.ndf (Tablas con temperatura media (4): TAZA-DE-INTERES, GARANTIAS/ALYCs, GARANTIASDISPONIBLIDAD, BROKER/COMISIONES)
- C:\BDD-PYME\SSD(G) : Para TA01.ndf (Tablas con temperatura alta (2): OPERACIONES, OPERACIONES_AUDITORIA)
- C:\BDD-PYME\SSD(H) : Para TA02.ndf (Tablas con temperatura alta (2): OPERACIONES, OPERACIONES_AUDITORIA)


El criterio que utlizamos para dividir los archivos es:
- Tablas con una sola clave primaria : Temperatura Baja.
- Tablas con clave compuesta: Temperatura Media.
- Tablas de Operaciones: Temperatura Alta.


# Modelo de datos: Financiamiento de PyMEs

Este documento explica los actores, los instrumentos y las reglas de negocio del sistema de un Broker que intermedia el financiamiento de PyMEs en el mercado de capitales.

---

## 1. Actores y flujos de información

El sistema intercambia información clave con cuatro actores principales.

### 1.1 PyMEs (Clientes)

Son las empresas que **inician el flujo de datos**. Ingresan solicitudes de financiamiento por alguno de estos dos medios:

- Descuento de un cheque de pago diferido.
- Toma de una caución.

El sistema procesa la solicitud y les devuelve la **liquidación** correspondiente, con el detalle de los costos (tasas, comisiones, gastos).

### 1.2 ALyCs (Agentes de Liquidación y Compensación)

El Broker opera como intermediario (similar a un Agente Productor) y **no tiene un ID o clave de liquidación propia** en el mercado general. Por eso, la comunicación con las plataformas de las ALyCs es **obligatoria**. A través de ese canal se:

- **Registran formalmente** las operaciones.
- **Validan los cupos de crédito**.
- **Consulta la disponibilidad de fondos**.

### 1.3 SGRs (Sociedades de Garantía Recíproca)

Son **proveedores externos de avales**. El sistema interactúa con ellas para verificar que los Cheques de Pago Diferido tengan el respaldo necesario **antes de enviarlos a negociación**. Contar con el aval permite acceder a **tasas de interés más bajas**.

### 1.4 Broker

Es el agente que actúa de **intermediario entre PyMEs, SGRs y ALyCs**. El flujo es el siguiente:

1. Recibe el pedido de financiación de la PyME.
2. Según el instrumento elegido:
   - **CPDG**: interactúa con una SGR para validar la garantía del cheque.
   - **Caución**: no interviene la SGR.
3. Interactúa con las ALyCs para conseguir un inversionista en algún mercado regulado por la CNV (Comisión Nacional de Valores).

```
PyME ──► Broker ──► SGR (solo CPDG) ──► ALyC ──► Inversionista / Mercado (CNV)
  ◄── liquidación ◄──────────────────────────────┘
```

---

## 2. Instrumentos

### 2.1 Caución

Funciona como un **préstamo a corto plazo**.

- **No requiere SGR**: el mercado toma activos (por ejemplo, acciones) como garantía.
- Al vencer el plazo se hace el **intercambio inverso**: se devuelve el efectivo más un interés (ejemplo: tasa del 25%).
- Si el tomador **incumple**, la garantía se **ejecuta automáticamente**, de modo que quien prestó el dinero no sufre pérdidas y recupera su capital.

### 2.2 CPDG (Cheque de Pago Diferido Garantizado)

Es el cheque que cuenta con el **aval de una SGR o de una entidad financiera**, lo que asegura su cobro al vencimiento. Ese respaldo es lo que lo hace atractivo para los inversionistas y permite obtener mejores tasas.

## 3. Resumen de relaciones (según el modelo implementado)

La tabla **OPERACIONES** es el centro del modelo: concentra las claves foráneas hacia PyME, ALyC, Broker, Instrumento y SGR.

| Tabla "uno" | Tabla "muchos" | Clave de la relación | Qué representa |
|---|---|---|---|
| BROKERS | OPERACIONES | ID-BROKER | Un Broker realiza muchas operaciones |
| PYMES | OPERACIONES | CUIT-PYME | Una PyME puede tener muchas operaciones |
| ALYCs | OPERACIONES | ID-ALYC | Cada operación se registra con una sola ALyC |
| INSTRUMENTOS | OPERACIONES | ID-INSTRUMENTO | Cada operación usa un solo instrumento |
| SGRs | OPERACIONES | ID-SGR | Una SGR puede avalar muchas operaciones |
| PYMES | GARANTIAS | CUIT-PYME | Cada garantía pertenece a una PyME |
| GARANTIAS | GARANTIASDISPONIBLIDAD | ID-GARANTIA | Una garantía tiene muchos períodos de disponibilidad/uso |
| OPERACIONES | GARANTIASDISPONIBLIDAD | ID-OPERACION | Cada período queda asociado a la operación que usa la garantía |
| INSTRUMENTOS | TAZA-DE-INTERES | ID-INSTRUMENTO | Un instrumento tiene un historial de tasas |
| BROKERS | BROKER/COMISIONES | ID-BROKER | Un Broker tiene un historial de comisiones |
| ALYCs ↔ GARANTIAS | (relación N a N) | tabla GARANTIAS/ALYCs | Una garantía puede estar vinculada a varias ALyCs y viceversa |
| OPERACIONES_AUDITORIA | — | — | Tabla de log, sin relaciones por clave foránea |

## 4. Entidades del modelo (detalle por tabla)

> Los nombres respetan los del diagrama (por ejemplo, `TAZA-DE-INTERES` y `GARANTIASDISPONIBLIDAD`). **PK** = clave primaria, **FK** = clave foránea.

### 4.1 PYMES

Empresas clientes del Broker.

| Campo | Clave | Descripción |
|---|---|---|
| CUIT-PYME | PK | Identificador fiscal de la PyME |
| RazonSocial | | Razón social |
| Direccion | | Domicilio de la empresa |

### 4.2 BROKERS

Intermediarios que gestionan las operaciones.

| Campo | Clave | Descripción |
|---|---|---|
| ID-BROKER | PK | Identificador del Broker |
| NombreBroker | | Nombre del Broker |

### 4.3 BROKER/COMISIONES

Historial de la comisión que cobra cada Broker.

| Campo | Clave | Descripción |
|---|---|---|
| ID-BROKER | PK, FK | Broker al que corresponde la comisión |
| FechaUltimaActualizacion | PK | Fecha desde la que rige ese porcentaje |
| PorcentajeComision | | Porcentaje de comisión vigente |

### 4.4 ALYCs

Agentes de Liquidación y Compensación con los que se registran las operaciones.

| Campo | Clave | Descripción |
|---|---|---|
| ID-ALYCS | PK | Identificador de la ALyC |
| NombreALYC | | Nombre de la ALyC |

### 4.5 SGRs

Sociedades de Garantía Recíproca que otorgan los avales.

| Campo | Clave | Descripción |
|---|---|---|
| ID-SGR | PK | Identificador de la SGR |
| Nombre-SGR | | Nombre de la SGR |

### 4.6 INSTRUMENTOS

Tipos de instrumento de financiamiento (CPDG o Caución).

| Campo | Clave | Descripción |
|---|---|---|
| ID-INSTRUMENTO | PK | Identificador del instrumento |
| TipoInstrumento | | Tipo: Cheque de Pago Diferido Garantizado o Caución |

### 4.7 TAZA-DE-INTERES

Historial de tasas de interés por instrumento.

| Campo | Clave | Descripción |
|---|---|---|
| ID-INSTRUMENTO | PK, FK | Instrumento al que aplica la tasa |
| FechaUltimaActualizacion | PK | Fecha desde la que rige esa tasa |
| TazaDeInteres | | Valor de la tasa (por ejemplo, 25%) |

### 4.8 OPERACIONES

Núcleo del modelo: cada operación de financiamiento gestionada por el Broker.

| Campo | Clave | Descripción |
|---|---|---|
| ID-OPERACION | PK | Identificador de la operación |
| CUIT-PYME | FK | PyME que solicita el financiamiento |
| ID-ALYC | FK | ALyC con la que se registra la operación |
| ID-BROKER | FK | Broker que la gestiona |
| ID-INSTRUMENTO | FK | Instrumento utilizado (CPDG o Caución) |
| ID-SGR | FK | SGR que avala (aplica a CPDG; en Caución no interviene) |
| Estado | | Estado de la operación (por ejemplo, abierta/cerrada) |
| MontoOperacion | | Monto financiado |

### 4.9 GARANTIAS

Garantías que posee cada PyME.

| Campo | Clave | Descripción |
|---|---|---|
| ID-GARANTIA | PK | Identificador de la garantía |
| CUIT-PYME | FK | PyME dueña de la garantía |
| DescripcionGarantia | | Descripción (hipoteca, plazo fijo, títulos, acciones, etc.) |
| MontoGarantia | | Valor de la garantía |

### 4.10 GARANTIASDISPONIBLIDAD

Controla cuándo una garantía está comprometida en una operación y cuándo vuelve a estar libre (regla 3.8).

| Campo | Clave | Descripción |
|---|---|---|
| ID-GARANTIA | PK, FK | Garantía a la que refiere el período |
| FechaDesde | PK | Inicio del período |
| FechaHasta | PK | Fin del período |
| ID-OPERACION | FK | Operación que utiliza la garantía en ese período |

### 4.11 GARANTIAS/ALYCs

Tabla intermedia de la relación N a N entre garantías y ALyCs.

| Campo | Clave | Descripción |
|---|---|---|
| ID-ALYC | PK, FK | ALyC vinculada |
| ID-GARANTIA | PK, FK | Garantía vinculada |

### 4.12 OPERACIONES_AUDITORIA

Registro histórico de acciones sobre las operaciones. No tiene claves foráneas declaradas.

| Campo | Clave | Descripción |
|---|---|---|
| ID-OPERACION | | Operación auditada |
| FechaOperacion | | Fecha en que ocurrió la acción |
| MontoOperacion | | Monto de la operación al momento de la acción |
| TipoAccion | | Tipo de acción registrada (por ejemplo, alta, modificación, baja) |