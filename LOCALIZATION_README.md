# JSON-Based Localization System

This project implements a comprehensive JSON-based localization system integrated with ABP Framework.

## Features

- **JSON-Only Storage**: Translations are stored in static JSON files, not in the database.
- **Culture Resolution**: Supports Query String, Cookie, User Setting, Tenant Setting, and Global Setting.
- **Hierarchical Settings**: Culture preferences are stored using ABP Settings (User -> Tenant -> Global).
- **Distributed Caching**: Localization dictionaries are cached for performance.
- **FluentValidation Integration**: Validation messages are fully localized.

## Usage

### 1. Adding Translations

Add your translation key-value pairs to the JSON files located in `Project.WebApi/Localization/Project/`.

**en.json**:
```json
{
  "culture": "en",
  "texts": {
    "Welcome": "Welcome",
    "User:UsernameRequired": "Username is required"
  }
}
```

**fr.json**:
```json
{
  "culture": "fr",
  "texts": {
    "Welcome": "Bienvenue",
    "User:UsernameRequired": "Le nom d'utilisateur est requis"
  }
}
```

### 2. Using Localization in Code

#### In AppServices (inheriting from `AppServiceBase`)

Use the `L()` helper method:

```csharp
public async Task DoSomething()
{
    var message = await L("Welcome");
    throw new UserFriendlyException(await L("User:UsernameRequired"));
}
```

#### In Validators

Use the localization key directly in `WithMessage`:

```csharp
RuleFor(x => x.UserName)
    .NotEmpty().WithMessage("User:UsernameRequired");
```

#### In Other Services

Inject `ILocalizationManager` and use `GetStringAsync`:

```csharp
public class MyService
{
    private readonly ILocalizationManager _localizationManager;

    public MyService(ILocalizationManager localizationManager)
    {
        _localizationManager = localizationManager;
    }

    public async Task Method()
    {
        var text = await _localizationManager.GetStringAsync(
            ProjectLocalizationResource.ResourceName, 
            "Welcome"
        );
    }
}
```

## Configuration

### Supported Cultures

Supported cultures are configured in the `App.Localization.SupportedCultures` setting (Default: "en,fr").

### Default Culture

The default culture is configured in `App.Localization.DefaultCulture` (Default: "en").

### Changing Culture

- **Query String**: `?culture=fr`
- **Cookie**: `.AspNetCore.Culture=c=fr|uic=fr`
- **Settings**: Update the `App.Localization.CurrentCulture` setting for the user or tenant.
