---
sidebar_position: 2
---

# Supported Filters

| Operator  | Linq                             | Applicable  |
| --------- | -------------------------------- | ----------- |
| =         | ==                               | Primitive\* |
| !=        | !=                               | Primitive\* |
| >         | >                                | Primitive\* |
| >=        | >=                               | Primitive\* |
| <         | <                                | Primitive\* |
| <=        | <=                               | Primitive\* |
| contains  | string.Contains()                | Primitive\* |
| !contains | !string.Contains()               | Primitive\* |
| and       | &&                               | -           |
| or        | \|\|                             | -           |
| in        | values.Contains(fieldValue)      | Primitive\* |
| !in       | !values.Contains(fieldValue)     | Primitive\* |
| any       | !Collection.Any(childExpression) | Collection  |
| all       | !Collection.All(childExpression) | Collection  |

\* Primitive - primitive types + string + DateTime + DateTimeOffset
