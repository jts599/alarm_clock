# Documentation Standards

This document defines the documentation standards for the Alarm Clock project. Use this as a reference when adding or updating code documentation.

## General Principles

- **Comprehensive**: Every public class, interface, method, and property must have XML documentation
- **Clear Purpose**: Explain what the code does and why it exists
- **Parameter Details**: Document all parameters with their purpose, valid ranges, and constraints
- **Exception Documentation**: List all exceptions that can be thrown and under what conditions
- **Usage Context**: Include remarks sections for complex implementations explaining usage patterns

## XML Documentation Format (C#)

### Classes and Interfaces

```csharp
/// <summary>
/// Brief one-line description of what this class/interface does.
/// </summary>
/// <remarks>
/// More detailed explanation including:
/// - Use cases and scenarios
/// - Key design decisions
/// - Important implementation details
/// - Related classes or patterns
/// </remarks>
public class ExampleClass
{
}
```

### Methods

```csharp
/// <summary>
/// Brief description of what this method does (starts with a verb).
/// </summary>
/// <param name="parameterName">Description of the parameter, including valid values, ranges, and constraints.</param>
/// <returns>Description of what is returned, including type and meaning.</returns>
/// <exception cref="ExceptionType">Conditions under which this exception is thrown.</exception>
/// <remarks>
/// Optional: Additional context, usage examples, or important notes.
/// </remarks>
public ReturnType MethodName(ParameterType parameterName)
{
}
```

### Properties

```csharp
/// <summary>
/// Description of what this property represents.
/// </summary>
/// <value>Description of the value type and meaning.</value>
/// <remarks>
/// Optional: When the property is set, side effects, or constraints.
/// </remarks>
public PropertyType PropertyName { get; set; }
```

### Fields

```csharp
/// <summary>
/// Description of what this field stores and its purpose.
/// </summary>
private FieldType _fieldName;
```

## TypeScript/JavaScript Documentation (JSDoc)

### Classes and Interfaces

````typescript
/**
 * Brief one-line description of what this class/interface does.
 *
 * More detailed explanation including:
 * - Use cases and scenarios
 * - Key design decisions
 * - Important implementation details
 *
 * @example
 * ```typescript
 * const example = new ExampleClass();
 * ```
 */
export class ExampleClass {}
````

### Functions and Methods

````typescript
/**
 * Brief description of what this function does (starts with a verb).
 *
 * @param parameterName - Description of the parameter, including valid values and constraints
 * @returns Description of what is returned
 * @throws {ErrorType} Conditions under which this error is thrown
 *
 * @example
 * ```typescript
 * const result = functionName(value);
 * ```
 */
function functionName(parameterName: Type): ReturnType {}
````

## Documentation Checklist

When documenting code, ensure:

- [ ] **Summary**: One-line summary present and clear
- [ ] **Purpose**: Why this code exists is explained
- [ ] **Parameters**: All parameters documented with types and constraints
- [ ] **Return Values**: What is returned and what it means
- [ ] **Exceptions**: All thrown exceptions documented with conditions
- [ ] **Remarks**: Complex logic has additional explanation
- [ ] **Examples**: Public APIs include usage examples where helpful
- [ ] **Links**: Related classes/methods referenced using proper XML tags

## AI Assistant Prompt Template

When asking an AI to add documentation to code, use this template:

```
Add comprehensive documentation to [file/class/method name] following these standards:

1. Every public member needs XML documentation (C#) or JSDoc (TypeScript)
2. Include:
   - Clear summary describing purpose
   - All parameters with types, purpose, and constraints
   - Return value descriptions
   - All possible exceptions with conditions
   - Remarks section for complex implementations
3. Use proper XML tags: <summary>, <param>, <returns>, <exception>, <remarks>, <value>
4. Start method descriptions with action verbs
5. Be specific about valid ranges, constraints, and side effects
6. Reference related classes using proper tags

Example format: [Include relevant example from above based on language]
```

## Quick Reference Examples

### Simple Method

```csharp
/// <summary>
/// Calculates the total price including tax.
/// </summary>
/// <param name="basePrice">The base price before tax (must be positive).</param>
/// <param name="taxRate">The tax rate as a decimal (e.g., 0.08 for 8%).</param>
/// <returns>The total price including tax.</returns>
/// <exception cref="ArgumentOutOfRangeException">Thrown when basePrice or taxRate is negative.</exception>
public decimal CalculateTotal(decimal basePrice, decimal taxRate)
```

### Interface Implementation

```csharp
/// <summary>
/// Interface for services that control hardware brightness.
/// Provides methods to read and adjust screen brightness levels.
/// </summary>
/// <remarks>
/// Implementations may target different hardware:
/// - Raspberry Pi backlight controls
/// - Mock implementations for testing
/// - Alternative display hardware
/// </remarks>
public interface IBrightnessService
```

### Configuration Class

```csharp
/// <summary>
/// Configuration options for application runtime behavior.
/// </summary>
/// <remarks>
/// This configuration is loaded from appsettings.json and can vary between
/// Development and Production environments. Changes require application restart.
/// </remarks>
public class RunConfiguration
```

## Maintenance

- Review documentation during code reviews
- Update documentation when behavior changes
- Add examples for commonly misused APIs
- Keep documentation in sync with code changes
