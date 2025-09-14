# Menu Orchestration System Design Document

<details>
<summary><strong>📋 Overview</strong></summary>

This document outlines a comprehensive menu orchestration system design for the CabApp console application. The design focuses on creating a flexible, maintainable, and extensible framework that integrates seamlessly with the existing dependency injection, logging, and data storage infrastructure.

</details>

<details>
<summary><strong>🎯 Design Philosophy</strong></summary>

### Core Principles

1. **Separation of Concerns** - Clear boundaries between UI, business logic, and data access
2. **Dependency Injection** - Leverage existing DI container for loose coupling
3. **Async/Await Pattern** - Modern C# patterns for better performance and responsiveness
4. **Extensibility** - Easy to add new menu actions and navigation flows
5. **Testability** - Interface-based design for easy unit testing
6. **Consistency** - Uniform patterns across all menu components

</details>

<details>
<summary><strong>🏗️ Architecture Overview</strong></summary>

```
┌─────────────────────────────────────────────────────────────┐
│                    Menu Orchestration System                │
├─────────────────────────────────────────────────────────────┤
│  IMenuOrchestrator  │  IMenuService  │  IMenuAction         │
│  (Flow Control)     │  (UI Operations) │  (Individual Items)│
├─────────────────────────────────────────────────────────────┤
│  MenuOrchestrator   │  MenuService   │  BaseMenuAction      │
│  (Implementation)   │  (Implementation) │  (Base Class)     │
├─────────────────────────────────────────────────────────────┤
│  ViewCabsAction     │  AddCabAction  │  ViewTripsAction     │
│  AddTripAction      │  ViewDriversAction │  ExitAction      │
├─────────────────────────────────────────────────────────────┤
│  IDataStore         │  IAppLogger    │  Data Models         │
│  (Existing)         │  (Existing)    │  (Existing)          │
└─────────────────────────────────────────────────────────────┘
```

</details>

<details>
<summary><strong>🔌 Core Interfaces</strong></summary>

### 1. IMenuOrchestrator

```csharp
public interface IMenuOrchestrator
{
    Task StartAsync();
    Task ShowMainMenuAsync();
    Task ShowMenuAsync(string menuType = "main");
    Task HandleUserInputAsync(string input);
    Task HandleUserInputAsync(string input, List<IMenuAction> actions, string currentMenu);
    Task ExitAsync();
    
    // Navigation methods for sub-menu support
    void NavigateToMenu(string menuType);
    void NavigateBack();
    void NavigateToMain();
}
```

**Design Rationale:**
- **Async Methods**: All methods are async to support non-blocking operations and better user experience
- **Single Responsibility**: Each method has a clear, focused purpose
- **Lifecycle Management**: Clear start/exit lifecycle for the menu system
- **Input Handling**: Centralized input processing for consistent behavior

### 2. IMenuService

```csharp
public interface IMenuService
{
    Task DisplayMenuAsync(string title, List<IMenuAction> actions);
    Task<string> GetUserInputAsync(string prompt = "Enter your choice: ");
    Task DisplayMessageAsync(string message, bool isError = false);
    Task ClearScreenAsync();
    Task WaitForUserAsync(string message = "Press any key to continue...");
}
```

**Design Rationale:**
- **UI Abstraction**: Separates UI concerns from business logic
- **Consistent Interface**: Standardized methods for all UI operations
- **Error Handling**: Built-in support for error message display
- **User Experience**: Methods for screen management and user interaction
- **Extensibility**: Easy to swap implementations (console, web, etc.)

### 3. IMenuAction

```csharp
public interface IMenuAction
{
    string Title { get; }
    string Description { get; }
    char Key { get; }
    bool IsEnabled { get; }
    Task<bool> ExecuteAsync();
}
```

**Design Rationale:**
- **Command Pattern**: Each menu item is a command that can be executed
- **Metadata Properties**: Title, description, and key for menu display
- **State Management**: IsEnabled property for dynamic menu behavior
- **Return Value**: Boolean return indicates success/failure for error handling
- **Async Execution**: Supports long-running operations

</details>

<details>
<summary><strong>⚙️ Implementation Classes</strong></summary>

### 1. MenuOrchestrator

**Key Design Decisions:**

```csharp
public class MenuOrchestrator : IMenuOrchestrator
{
    private readonly IMenuService _menuService;
    private readonly IDataStore _dataStore;
    private readonly IAppLogger _logger;
    private bool _isRunning = true;
    
    // Constructor injection for dependencies
    public MenuOrchestrator(IMenuService menuService, IDataStore dataStore, IAppLogger logger)
    {
        _menuService = menuService;
        _dataStore = dataStore;
        _logger = logger;
    }
}
```

**Rationale:**
- **Dependency Injection**: All dependencies injected through constructor
- **State Management**: `_isRunning` flag for clean application lifecycle
- **Logging Integration**: Uses existing IAppLogger for consistent logging
- **Data Access**: Leverages existing IDataStore for data operations

### 2. MenuService

**Key Design Decisions:**

```csharp
public class MenuService : IMenuService
{
    private readonly IAppLogger _logger;
    
    public async Task DisplayMenuAsync(string title, List<IMenuAction> actions)
    {
        await ClearScreenAsync();
        
        // Format menu with consistent styling
        Console.WriteLine("=".PadRight(60, '='));
        Console.WriteLine($"  {title.ToUpper()}");
        Console.WriteLine("=".PadRight(60, '='));
        
        // Display only enabled actions
        var enabledActions = actions.Where(a => a.IsEnabled).ToList();
        foreach (var action in enabledActions)
        {
            Console.WriteLine($"  {action.Key}. {action.Title}");
            if (!string.IsNullOrEmpty(action.Description))
            {
                Console.WriteLine($"     {action.Description}");
            }
        }
    }
}
```

**Rationale:**
- **Consistent Formatting**: Standardized menu appearance
- **Dynamic Filtering**: Only shows enabled actions
- **User-Friendly**: Clear visual hierarchy and descriptions
- **Logging**: Tracks user interactions for debugging

### 3. Direct IMenuAction Implementation

**Key Design Decision:**
Instead of using a base class, each menu action directly implements `IMenuAction` interface. This provides more flexibility and avoids inheritance complexity.

**Rationale:**
- **No Inheritance Overhead**: Direct implementation without base class dependencies
- **Maximum Flexibility**: Each action can have its own constructor and dependencies
- **Simpler DI**: No need to inject common dependencies through base class
- **Clear Interface Contract**: Direct adherence to IMenuAction interface

</details>

<details>
<summary><strong>🎬 Menu Actions Implementation</strong></summary>

### 1. ViewCabsAction

```csharp
public class ViewCabsAction : IMenuAction
{
    private readonly IDataStore _dataStore;
    private readonly IMenuService _menuService;
    private readonly IAppLogger _logger;
    
    public ViewCabsAction(IDataStore dataStore, IMenuService menuService, IAppLogger logger)
    {
        _dataStore = dataStore;
        _menuService = menuService;
        _logger = logger;
    }
    
    public string Title => "View All Cabs";
    public string Description => "Display all cabs in the system with their details";
    public char Key => 'V';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        try
        {
            _logger.LogInfo("Executing ViewCabsAction");
            var cabs = await _dataStore.GetAllCabsAsync();
            await _menuService.ClearScreenAsync();
            await _menuService.DisplayMessageAsync($"Found {cabs.Count} cabs in the system", false);
            
            // Display formatted table
            Console.WriteLine($"{"ID",-5} {"Model",-15} {"Driver",-20} {"Status",-15}");
            foreach (var cab in cabs)
            {
                Console.WriteLine($"{cab.Id,-5} {cab.CarInfo?.ModelName,-15} " +
                                $"{cab.DriverInfo?.FirstName + " " + cab.DriverInfo?.LastName,-20} " +
                                $"{cab.CurrentWorkState,-15}");
            }
            
            await _menuService.WaitForUserAsync();
            _logger.LogInfo("ViewCabsAction completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in ViewCabsAction", ex);
            await _menuService.DisplayMessageAsync($"Error viewing cabs: {ex.Message}", true);
            await _menuService.WaitForUserAsync();
            return false;
        }
    }
}
```

**Design Rationale:**
- **Error Handling**: Comprehensive try-catch with user feedback
- **Data Formatting**: Consistent table formatting for readability
- **User Experience**: Clear success/error messages
- **Logging**: Errors are logged for debugging
- **Return Value**: Boolean indicates success for orchestrator

### 2. AddCabAction

```csharp
public class AddCabAction : IMenuAction
{
    private readonly IDataStore _dataStore;
    private readonly IMenuService _menuService;
    private readonly IAppLogger _logger;
    
    public AddCabAction(IDataStore dataStore, IMenuService menuService, IAppLogger logger)
    {
        _dataStore = dataStore;
        _menuService = menuService;
        _logger = logger;
    }
    
    public string Title => "Add New Cab";
    public string Description => "Add a new cab to the system";
    public char Key => 'A';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        try
        {
            _logger.LogInfo("Executing AddCabAction");
            
            // Collect user input
            var modelName = await _menuService.GetUserInputAsync("Enter car model: ");
            var firstName = await _menuService.GetUserInputAsync("Enter driver first name: ");
            var lastName = await _menuService.GetUserInputAsync("Enter driver last name: ");
            
            // Validation
            if (string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(firstName))
            {
                await _menuService.DisplayMessageAsync("Required fields cannot be empty.", true);
                _logger.LogWarning("AddCabAction failed: Required fields empty");
                return false;
            }
            
            // Create and save cab
            var newCab = new CabDetails
            {
                CarInfo = new CarInfo { ModelName = modelName },
                DriverInfo = new DriverInfo { FirstName = firstName, LastName = lastName },
                CurrentWorkState = WorkState.IDLE
            };
            
            await _dataStore.AddCabAsync(newCab);
            await _menuService.DisplayMessageAsync($"Cab added successfully with ID: {newCab.Id}", false);
            _logger.LogInfo($"AddCabAction completed successfully. New cab ID: {newCab.Id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in AddCabAction", ex);
            await _menuService.DisplayMessageAsync($"Error adding cab: {ex.Message}", true);
            return false;
        }
    }
}
```

**Design Rationale:**
- **Input Validation**: Client-side validation before data operations
- **User Guidance**: Clear prompts for required information
- **Data Creation**: Proper object construction with default values
- **Success Feedback**: Confirmation with generated ID
- **Error Recovery**: Graceful error handling with user notification

### 3. Additional Sub-Menu Actions

**EditCabAction:**
```csharp
public class EditCabAction : IMenuAction
{
    private readonly IDataStore _dataStore;
    private readonly IMenuService _menuService;
    private readonly IAppLogger _logger;
    
    public EditCabAction(IDataStore dataStore, IMenuService menuService, IAppLogger logger)
    {
        _dataStore = dataStore;
        _menuService = menuService;
        _logger = logger;
    }
    
    public string Title => "Edit Cab";
    public string Description => "Modify existing cab information";
    public char Key => 'E';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        try
        {
            _logger.LogInfo("Executing EditCabAction");
            var cabIdInput = await _menuService.GetUserInputAsync("Enter cab ID to edit: ");
            
            if (!int.TryParse(cabIdInput, out int cabId))
            {
                await _menuService.DisplayMessageAsync("Invalid cab ID.", true);
                return false;
            }
            
            var cabs = await _dataStore.GetAllCabsAsync();
            var cab = cabs.FirstOrDefault(c => c.Id == cabId);
            
            if (cab == null)
            {
                await _menuService.DisplayMessageAsync("Cab not found.", true);
                return false;
            }
            
            // Get new information
            var newModel = await _menuService.GetUserInputAsync($"Enter new model (current: {cab.CarInfo?.ModelName}): ");
            var newFirstName = await _menuService.GetUserInputAsync($"Enter new first name (current: {cab.DriverInfo?.FirstName}): ");
            
            if (!string.IsNullOrEmpty(newModel))
                cab.CarInfo.ModelName = newModel;
            if (!string.IsNullOrEmpty(newFirstName))
                cab.DriverInfo.FirstName = newFirstName;
            
            await _dataStore.WriteData(cabs, "cabs");
            await _menuService.DisplayMessageAsync("Cab updated successfully!", false);
            _logger.LogInfo($"EditCabAction completed successfully for cab ID: {cabId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in EditCabAction", ex);
            await _menuService.DisplayMessageAsync($"Error editing cab: {ex.Message}", true);
            return false;
        }
    }
}
```

**DeleteCabAction:**
```csharp
public class DeleteCabAction : IMenuAction
{
    private readonly IDataStore _dataStore;
    private readonly IMenuService _menuService;
    private readonly IAppLogger _logger;
    
    public DeleteCabAction(IDataStore dataStore, IMenuService menuService, IAppLogger logger)
    {
        _dataStore = dataStore;
        _menuService = menuService;
        _logger = logger;
    }
    
    public string Title => "Delete Cab";
    public string Description => "Remove a cab from the system";
    public char Key => 'D';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        try
        {
            _logger.LogInfo("Executing DeleteCabAction");
            var cabIdInput = await _menuService.GetUserInputAsync("Enter cab ID to delete: ");
            
            if (!int.TryParse(cabIdInput, out int cabId))
            {
                await _menuService.DisplayMessageAsync("Invalid cab ID.", true);
                return false;
            }
            
            var cabs = await _dataStore.GetAllCabsAsync();
            var cab = cabs.FirstOrDefault(c => c.Id == cabId);
            
            if (cab == null)
            {
                await _menuService.DisplayMessageAsync("Cab not found.", true);
                return false;
            }
            
            var confirm = await _menuService.GetUserInputAsync($"Are you sure you want to delete cab {cabId}? (y/n): ");
            if (confirm.ToLower() != "y")
            {
                await _menuService.DisplayMessageAsync("Deletion cancelled.", false);
                return false;
            }
            
            cabs.Remove(cab);
            await _dataStore.WriteData(cabs, "cabs");
            await _menuService.DisplayMessageAsync("Cab deleted successfully!", false);
            _logger.LogInfo($"DeleteCabAction completed successfully for cab ID: {cabId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in DeleteCabAction", ex);
            await _menuService.DisplayMessageAsync($"Error deleting cab: {ex.Message}", true);
            return false;
        }
    }
}
```

**SearchCabsAction:**
```csharp
public class SearchCabsAction : IMenuAction
{
    private readonly IDataStore _dataStore;
    private readonly IMenuService _menuService;
    private readonly IAppLogger _logger;
    
    public SearchCabsAction(IDataStore dataStore, IMenuService menuService, IAppLogger logger)
    {
        _dataStore = dataStore;
        _menuService = menuService;
        _logger = logger;
    }
    
    public string Title => "Search Cabs";
    public string Description => "Find cabs by model or driver name";
    public char Key => 'S';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        try
        {
            _logger.LogInfo("Executing SearchCabsAction");
            var searchTerm = await _menuService.GetUserInputAsync("Enter search term (model or driver name): ");
            
            if (string.IsNullOrEmpty(searchTerm))
            {
                await _menuService.DisplayMessageAsync("Search term cannot be empty.", true);
                return false;
            }
            
            var cabs = await _dataStore.GetAllCabsAsync();
            var filteredCabs = cabs.Where(c => 
                c.CarInfo?.ModelName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                c.DriverInfo?.FirstName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                c.DriverInfo?.LastName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();
            
            await _menuService.ClearScreenAsync();
            await _menuService.DisplayMessageAsync($"Found {filteredCabs.Count} cabs matching '{searchTerm}'", false);
            
            if (filteredCabs.Any())
            {
                Console.WriteLine($"{"ID",-5} {"Model",-15} {"Driver",-20} {"Status",-15}");
                foreach (var cab in filteredCabs)
                {
                    Console.WriteLine($"{cab.Id,-5} {cab.CarInfo?.ModelName,-15} " +
                                    $"{cab.DriverInfo?.FirstName + " " + cab.DriverInfo?.LastName,-20} " +
                                    $"{cab.CurrentWorkState,-15}");
                }
            }
            
            await _menuService.WaitForUserAsync();
            _logger.LogInfo($"SearchCabsAction completed successfully. Found {filteredCabs.Count} results");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in SearchCabsAction", ex);
            await _menuService.DisplayMessageAsync($"Error searching cabs: {ex.Message}", true);
            return false;
        }
    }
}
```

**ExitAction:**
```csharp
public class ExitAction : IMenuAction
{
    private readonly IMenuService _menuService;
    private readonly IAppLogger _logger;
    
    public ExitAction(IMenuService menuService, IAppLogger logger)
    {
        _menuService = menuService;
        _logger = logger;
    }
    
    public string Title => "Exit";
    public string Description => "Exit the application";
    public char Key => 'X';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        _logger.LogInfo("Executing ExitAction");
        await _menuService.DisplayMessageAsync("Thank you for using Cab Management System. Goodbye!", false);
        await _menuService.WaitForUserAsync();
        return true;
    }
}
```

**ViewTripsAction:**
```csharp
public class ViewTripsAction : IMenuAction
{
    private readonly IDataStore _dataStore;
    private readonly IMenuService _menuService;
    private readonly IAppLogger _logger;
    
    public ViewTripsAction(IDataStore dataStore, IMenuService menuService, IAppLogger logger)
    {
        _dataStore = dataStore;
        _menuService = menuService;
        _logger = logger;
    }
    
    public string Title => "View All Trips";
    public string Description => "Display all trips in the system";
    public char Key => 'V';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        try
        {
            _logger.LogInfo("Executing ViewTripsAction");
            var trips = await _dataStore.GetAllTripsAsync();
            await _menuService.ClearScreenAsync();
            await _menuService.DisplayMessageAsync($"Found {trips.Count} trips in the system", false);
            
            // Display formatted table
            Console.WriteLine($"{"ID",-5} {"From",-20} {"To",-20} {"Status",-15} {"Cab ID",-8}");
            foreach (var trip in trips)
            {
                Console.WriteLine($"{trip.Id,-5} {trip.PickupLocation,-20} {trip.DropoffLocation,-20} " +
                                $"{trip.Status,-15} {trip.CabId,-8}");
            }
            
            await _menuService.WaitForUserAsync();
            _logger.LogInfo("ViewTripsAction completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in ViewTripsAction", ex);
            await _menuService.DisplayMessageAsync($"Error viewing trips: {ex.Message}", true);
            await _menuService.WaitForUserAsync();
            return false;
        }
    }
}
```

</details>

<details>
<summary><strong>💾 Data Store Extensions</strong></summary>

### Extended IDataStore Interface

```csharp
public interface IDataStore
{
    // Existing generic methods
    Task<List<T>> GetData<T>(string storeName);
    Task WriteData<T>(List<T> data, string storeName);
    void ClearData(string storeName);
    
    // New domain-specific methods
    Task<List<CabDetails>> GetAllCabsAsync();
    Task AddCabAsync(CabDetails cab);
    Task<List<TripDetail>> GetAllTripsAsync();
    Task AddTripAsync(TripDetail trip);
    Task<List<DriverInfo>> GetAllDriversAsync();
    Task AddDriverAsync(DriverInfo driver);
}
```

**Design Rationale:**
- **Backward Compatibility**: Existing generic methods remain unchanged
- **Domain-Specific**: Methods tailored to business entities
- **Type Safety**: Strongly-typed methods prevent runtime errors
- **Consistency**: Uniform naming convention across all methods
- **Extensibility**: Easy to add new entity-specific methods

### DataStore Implementation

```csharp
public async Task AddCabAsync(CabDetails cab)
{
    var cabs = await GetAllCabsAsync();
    cab.Id = cabs.Count > 0 ? cabs.Max(c => c.Id) + 1 : 1;
    cabs.Add(cab);
    await WriteData(cabs, "cabs");
}
```

**Design Rationale:**
- **ID Generation**: Automatic ID assignment based on existing data
- **Atomic Operations**: Read-modify-write pattern for data consistency
- **File-Based Storage**: Leverages existing JSON file storage
- **Error Handling**: Inherits error handling from base WriteData method

</details>

<details>
<summary><strong>🏗️ Complete Interface and Class Implementations</strong></summary>

### 1. Complete IMenuOrchestrator Implementation

```csharp
public class MenuOrchestrator : IMenuOrchestrator
{
    private readonly IMenuService _menuService;
    private readonly IDataStore _dataStore;
    private readonly IAppLogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private bool _isRunning = true;
    private Stack<string> _menuStack = new Stack<string>();
    
    public MenuOrchestrator(IMenuService menuService, IDataStore dataStore, 
                           IAppLogger logger, IServiceProvider serviceProvider)
    {
        _menuService = menuService;
        _dataStore = dataStore;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    public async Task StartAsync()
    {
        _logger.LogInfo("MenuOrchestrator started");
        await ShowMenuAsync("main");
    }
    
    public async Task ShowMainMenuAsync()
    {
        await ShowMenuAsync("main");
    }
    
    public async Task ShowMenuAsync(string menuType = "main")
    {
        _menuStack.Push(menuType);
        
        while (_isRunning && _menuStack.Count > 0)
        {
            try
            {
                var currentMenu = _menuStack.Peek();
                var actions = GetActionsForMenu(currentMenu);
                
                await _menuService.DisplayMenuAsync(GetMenuTitle(currentMenu), actions);
                var userInput = await _menuService.GetUserInputAsync();
                await HandleUserInputAsync(userInput, actions, currentMenu);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in menu loop for {_menuStack.Peek()}", ex);
                await _menuService.DisplayMessageAsync("An error occurred. Please try again.", true);
                await _menuService.WaitForUserAsync();
            }
        }
    }
    
    public async Task HandleUserInputAsync(string input)
    {
        // Legacy method for backward compatibility
        await HandleUserInputAsync(input, CreateMainMenuActions(), "main");
    }
    
    public async Task HandleUserInputAsync(string input, List<IMenuAction> actions, string currentMenu)
    {
        if (string.IsNullOrEmpty(input))
        {
            await _menuService.DisplayMessageAsync("Please enter a valid choice.", true);
            return;
        }
        
        var choice = input.ToUpper().FirstOrDefault();
        var selectedAction = actions.FirstOrDefault(a => a.Key == choice && a.IsEnabled);
        
        if (selectedAction == null)
        {
            await _menuService.DisplayMessageAsync("Invalid choice. Please try again.", true);
            return;
        }
        
        try
        {
            _logger.LogInfo($"Executing action: {selectedAction.Title} in menu: {currentMenu}");
            var success = await selectedAction.ExecuteAsync();
            
            if (success)
            {
                _logger.LogInfo($"Action completed successfully: {selectedAction.Title}");
            }
            else
            {
                _logger.LogWarning($"Action completed with issues: {selectedAction.Title}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error executing action: {selectedAction.Title}", ex);
            await _menuService.DisplayMessageAsync($"Error executing {selectedAction.Title}: {ex.Message}", true);
        }
        
        await _menuService.WaitForUserAsync();
    }
    
    public async Task ExitAsync()
    {
        _logger.LogInfo("MenuOrchestrator exiting");
        _isRunning = false;
        await Task.CompletedTask;
    }
    
    // Navigation methods
    public void NavigateToMenu(string menuType)
    {
        _menuStack.Push(menuType);
    }
    
    public void NavigateBack()
    {
        if (_menuStack.Count > 1)
        {
            _menuStack.Pop();
        }
    }
    
    public void NavigateToMain()
    {
        _menuStack.Clear();
        _menuStack.Push("main");
    }
    
    // Helper methods
    private List<IMenuAction> CreateMainMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<CabMenuAction>(),
            _serviceProvider.GetRequiredService<DriverMenuAction>(),
            _serviceProvider.GetRequiredService<LocationMenuAction>(),
            _serviceProvider.GetRequiredService<TripMenuAction>(),
            _serviceProvider.GetRequiredService<ReportsMenuAction>(),
            _serviceProvider.GetRequiredService<ExitAction>()
        };
    }
    
    private List<IMenuAction> GetActionsForMenu(string menuType)
    {
        return menuType switch
        {
            "main" => CreateMainMenuActions(),
            "cabs" => CreateCabSubMenuActions(),
            "drivers" => CreateDriverSubMenuActions(),
            "locations" => CreateLocationSubMenuActions(),
            "trips" => CreateTripSubMenuActions(),
            "reports" => CreateReportsSubMenuActions(),
            _ => CreateMainMenuActions()
        };
    }
    
    private string GetMenuTitle(string menuType)
    {
        return menuType switch
        {
            "main" => "Cab Management System - Main Menu",
            "cabs" => "Cab Management",
            "drivers" => "Driver Management",
            "locations" => "Location Management",
            "trips" => "Trip Management",
            "reports" => "Reports & Analytics",
            _ => "Cab Management System"
        };
    }
    
    // Sub-menu action creation methods
    private List<IMenuAction> CreateCabSubMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<ViewCabsAction>(),
            _serviceProvider.GetRequiredService<AddCabAction>(),
            _serviceProvider.GetRequiredService<EditCabAction>(),
            _serviceProvider.GetRequiredService<DeleteCabAction>(),
            _serviceProvider.GetRequiredService<SearchCabsAction>(),
            _serviceProvider.GetRequiredService<BackToMainMenuAction>()
        };
    }
    
    private List<IMenuAction> CreateDriverSubMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<ViewDriversAction>(),
            _serviceProvider.GetRequiredService<AddDriverAction>(),
            _serviceProvider.GetRequiredService<EditDriverAction>(),
            _serviceProvider.GetRequiredService<DeleteDriverAction>(),
            _serviceProvider.GetRequiredService<SearchDriversAction>(),
            _serviceProvider.GetRequiredService<BackToMainMenuAction>()
        };
    }
    
    private List<IMenuAction> CreateLocationSubMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<ViewLocationsAction>(),
            _serviceProvider.GetRequiredService<AddLocationAction>(),
            _serviceProvider.GetRequiredService<EditLocationAction>(),
            _serviceProvider.GetRequiredService<DeleteLocationAction>(),
            _serviceProvider.GetRequiredService<SearchLocationsAction>(),
            _serviceProvider.GetRequiredService<BackToMainMenuAction>()
        };
    }
    
    private List<IMenuAction> CreateTripSubMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<ViewTripsAction>(),
            _serviceProvider.GetRequiredService<AddTripAction>(),
            _serviceProvider.GetRequiredService<EditTripAction>(),
            _serviceProvider.GetRequiredService<CompleteTripAction>(),
            _serviceProvider.GetRequiredService<CancelTripAction>(),
            _serviceProvider.GetRequiredService<SearchTripsAction>(),
            _serviceProvider.GetRequiredService<BackToMainMenuAction>()
        };
    }
    
    private List<IMenuAction> CreateReportsSubMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<DailyReportAction>(),
            _serviceProvider.GetRequiredService<WeeklyReportAction>(),
            _serviceProvider.GetRequiredService<MonthlyReportAction>(),
            _serviceProvider.GetRequiredService<DriverPerformanceReportAction>(),
            _serviceProvider.GetRequiredService<CabUtilizationReportAction>(),
            _serviceProvider.GetRequiredService<BackToMainMenuAction>()
        };
    }
}
```

### 2. Complete IMenuService Implementation

```csharp
public class MenuService : IMenuService
{
    private readonly IAppLogger _logger;
    
    public MenuService(IAppLogger logger)
    {
        _logger = logger;
    }
    
    public async Task DisplayMenuAsync(string title, List<IMenuAction> actions)
    {
        await ClearScreenAsync();
        
        // Format menu with consistent styling
        Console.WriteLine("=".PadRight(60, '='));
        Console.WriteLine($"  {title.ToUpper()}");
        Console.WriteLine("=".PadRight(60, '='));
        
        // Display only enabled actions
        var enabledActions = actions.Where(a => a.IsEnabled).ToList();
        foreach (var action in enabledActions)
        {
            Console.WriteLine($"  {action.Key}. {action.Title}");
            if (!string.IsNullOrEmpty(action.Description))
            {
                Console.WriteLine($"     {action.Description}");
            }
        }
        
        Console.WriteLine();
        _logger.LogInfo($"Displayed menu: {title} with {enabledActions.Count} actions");
    }
    
    public async Task<string> GetUserInputAsync(string prompt = "Enter your choice: ")
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        _logger.LogInfo($"User input received: {input}");
        return await Task.FromResult(input ?? string.Empty);
    }
    
    public async Task DisplayMessageAsync(string message, bool isError = false)
    {
        if (isError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {message}");
            Console.ResetColor();
            _logger.LogError($"Error message displayed: {message}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
            _logger.LogInfo($"Info message displayed: {message}");
        }
        
        await Task.CompletedTask;
    }
    
    public async Task ClearScreenAsync()
    {
        Console.Clear();
        _logger.LogDebug("Screen cleared");
        await Task.CompletedTask;
    }
    
    public async Task WaitForUserAsync(string message = "Press any key to continue...")
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.ReadKey();
        _logger.LogDebug("User acknowledged message");
        await Task.CompletedTask;
    }
}
```

### 3. Complete IDataStore Implementation

```csharp
public class DataStore : IDataStore
{
    private readonly string _dataDirectory;
    private readonly IAppLogger _logger;
    
    public DataStore(string dataDirectory, IAppLogger logger)
    {
        _dataDirectory = dataDirectory;
        _logger = logger;
        
        // Ensure data directory exists
        if (!Directory.Exists(_dataDirectory))
        {
            Directory.CreateDirectory(_dataDirectory);
        }
    }
    
    // Generic methods
    public async Task<List<T>> GetData<T>(string storeName)
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, $"{storeName}.json");
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }
            
            var json = await File.ReadAllTextAsync(filePath);
            var data = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            _logger.LogInfo($"Retrieved {data.Count} items from {storeName}");
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting data from {storeName}", ex);
            return new List<T>();
        }
    }
    
    public async Task WriteData<T>(List<T> data, string storeName)
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, $"{storeName}.json");
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
            _logger.LogInfo($"Wrote {data.Count} items to {storeName}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error writing data to {storeName}", ex);
            throw;
        }
    }
    
    public void ClearData(string storeName)
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, $"{storeName}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInfo($"Cleared data for {storeName}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error clearing data for {storeName}", ex);
            throw;
        }
    }
    
    // Domain-specific methods
    public async Task<List<CabDetails>> GetAllCabsAsync()
    {
        return await GetData<CabDetails>("cabs");
    }
    
    public async Task AddCabAsync(CabDetails cab)
    {
        var cabs = await GetAllCabsAsync();
        cab.Id = cabs.Count > 0 ? cabs.Max(c => c.Id) + 1 : 1;
        cabs.Add(cab);
        await WriteData(cabs, "cabs");
    }
    
    public async Task<List<TripDetail>> GetAllTripsAsync()
    {
        return await GetData<TripDetail>("trips");
    }
    
    public async Task AddTripAsync(TripDetail trip)
    {
        var trips = await GetAllTripsAsync();
        trip.Id = trips.Count > 0 ? trips.Max(t => t.Id) + 1 : 1;
        trips.Add(trip);
        await WriteData(trips, "trips");
    }
    
    public async Task<List<DriverInfo>> GetAllDriversAsync()
    {
        return await GetData<DriverInfo>("drivers");
    }
    
    public async Task AddDriverAsync(DriverInfo driver)
    {
        var drivers = await GetAllDriversAsync();
        driver.Id = drivers.Count > 0 ? drivers.Max(d => d.Id) + 1 : 1;
        drivers.Add(driver);
        await WriteData(drivers, "drivers");
    }
}
```

### 4. Complete IAppLogger Implementation

```csharp
public class AppLogger : IAppLogger
{
    private readonly string _logFilePath;
    
    public AppLogger()
    {
        var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
        
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd");
        _logFilePath = Path.Combine(logDirectory, $"app-{timestamp}.log");
    }
    
    public void LogInfo(string message)
    {
        Log("INFO", message);
    }
    
    public void LogError(string message, Exception? exception = null)
    {
        var fullMessage = exception != null ? $"{message}: {exception}" : message;
        Log("ERROR", fullMessage);
    }
    
    public void LogWarning(string message)
    {
        Log("WARNING", message);
    }
    
    public void LogDebug(string message)
    {
        Log("DEBUG", message);
    }
    
    private void Log(string level, string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var logEntry = $"[{timestamp}] [{level}] {message}";
        
        // Write to console
        Console.WriteLine(logEntry);
        
        // Write to file
        try
        {
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }
        catch
        {
            // Ignore file write errors to prevent logging failures
        }
    }
}
```

</details>

<details>
<summary><strong>🔧 Dependency Injection Configuration</strong></summary>

### Service Registration

```csharp
.ConfigureServices((hostContext, services) =>
{
    // Core Services Registration
    services.AddSingleton<IAppLogger, AppLogger>();
    services.AddSingleton<IDataStore, DataStore>(provider =>
    {
        var logger = provider.GetService<IAppLogger>();
        var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        return new DataStore(dataDirectory, logger!);
    });

    // Menu Services Registration
    services.AddSingleton<IMenuService, MenuService>();
    services.AddSingleton<IMenuOrchestrator, MenuOrchestrator>();

    // Menu Actions Registration - Direct IMenuAction implementations
    services.AddTransient<ViewCabsAction>();
    services.AddTransient<AddCabAction>();
    services.AddTransient<ViewTripsAction>();
    services.AddTransient<ExitAction>();

    // Application Registration
    services.AddSingleton<App>();
});
```

**Design Rationale:**
- **Singleton Services**: MenuService and MenuOrchestrator are stateless and can be singletons
- **Transient Actions**: Menu actions are stateless and can be created per request
- **Factory Pattern**: DataStore uses factory pattern for configuration
- **Lifetime Management**: Appropriate lifetimes for different service types

</details>

<details>
<summary><strong>🔄 Menu Action Execution Flow</strong></summary>

### How IMenuActions are Called from MenuOrchestrator

The `MenuOrchestrator` coordinates the execution of different `IMenuAction` implementations through a centralized flow. Here's how the system works:

#### 1. Menu Action Registration and Creation

```csharp
public class MenuOrchestrator : IMenuOrchestrator
{
    private readonly IMenuService _menuService;
    private readonly IDataStore _dataStore;
    private readonly IAppLogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private bool _isRunning = true;
    
    public MenuOrchestrator(IMenuService menuService, IDataStore dataStore, 
                           IAppLogger logger, IServiceProvider serviceProvider)
    {
        _menuService = menuService;
        _dataStore = dataStore;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    private List<IMenuAction> CreateMainMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<ViewCabsAction>(),
            _serviceProvider.GetRequiredService<AddCabAction>(),
            _serviceProvider.GetRequiredService<ViewTripsAction>(),
            _serviceProvider.GetRequiredService<AddTripAction>(),
            _serviceProvider.GetRequiredService<ViewDriversAction>(),
            _serviceProvider.GetRequiredService<ExitAction>()
        };
    }
}
```

#### 2. Main Menu Display and Action Selection

```csharp
public async Task ShowMainMenuAsync()
{
    while (_isRunning)
    {
        try
        {
            // Create fresh instances of menu actions
            var actions = CreateMainMenuActions();
            
            // Display the menu with all available actions
            await _menuService.DisplayMenuAsync("Cab Management System", actions);
            
            // Get user input
            var userInput = await _menuService.GetUserInputAsync();
            
            // Process the user's choice
            await HandleUserInputAsync(userInput, actions);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in main menu loop", ex);
            await _menuService.DisplayMessageAsync("An error occurred. Please try again.", true);
        }
    }
}
```

#### 3. User Input Processing and Action Execution

```csharp
public async Task HandleUserInputAsync(string input, List<IMenuAction> actions)
{
    if (string.IsNullOrEmpty(input))
    {
        await _menuService.DisplayMessageAsync("Please enter a valid choice.", true);
        return;
    }
    
    var choice = input.ToUpper().FirstOrDefault();
    
    // Find the action that matches the user's choice
    var selectedAction = actions.FirstOrDefault(a => a.Key == choice && a.IsEnabled);
    
    if (selectedAction == null)
    {
        await _menuService.DisplayMessageAsync("Invalid choice. Please try again.", true);
        return;
    }
    
    try
    {
        _logger.LogInfo($"Executing action: {selectedAction.Title}");
        
        // Execute the selected action
        var success = await selectedAction.ExecuteAsync();
        
        if (success)
        {
            _logger.LogInfo($"Action completed successfully: {selectedAction.Title}");
        }
        else
        {
            _logger.LogWarning($"Action completed with issues: {selectedAction.Title}");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error executing action: {selectedAction.Title}", ex);
        await _menuService.DisplayMessageAsync($"Error executing {selectedAction.Title}: {ex.Message}", true);
    }
    
    // Wait for user acknowledgment before returning to main menu
    await _menuService.WaitForUserAsync();
}
```

#### 4. Specific Action Execution Examples

**ViewCabsAction Execution Flow:**
```csharp
// User selects 'V' from main menu
// MenuOrchestrator finds ViewCabsAction with Key = 'V'
// Calls: await viewCabsAction.ExecuteAsync()

public override async Task<bool> ExecuteAsync()
{
    try
    {
        // Action-specific logic
        var cabs = await _dataStore.GetAllCabsAsync();
        await _menuService.ClearScreenAsync();
        await _menuService.DisplayMessageAsync($"Found {cabs.Count} cabs in the system", false);
        
        // Display formatted data
        Console.WriteLine($"{"ID",-5} {"Model",-15} {"Driver",-20} {"Status",-15}");
        foreach (var cab in cabs)
        {
            Console.WriteLine($"{cab.Id,-5} {cab.CarInfo?.ModelName,-15} " +
                            $"{cab.DriverInfo?.FirstName + " " + cab.DriverInfo?.LastName,-20} " +
                            $"{cab.CurrentWorkState,-15}");
        }
        
        await _menuService.WaitForUserAsync();
        return true; // Success
    }
    catch (Exception ex)
    {
        await _menuService.DisplayMessageAsync($"Error viewing cabs: {ex.Message}", true);
        return false; // Failure
    }
}
```

**AddCabAction Execution Flow:**
```csharp
// User selects 'A' from main menu
// MenuOrchestrator finds AddCabAction with Key = 'A'
// Calls: await addCabAction.ExecuteAsync()

public override async Task<bool> ExecuteAsync()
{
    try
    {
        // Collect user input through MenuService
        var modelName = await _menuService.GetUserInputAsync("Enter car model: ");
        var firstName = await _menuService.GetUserInputAsync("Enter driver first name: ");
        var lastName = await _menuService.GetUserInputAsync("Enter driver last name: ");
        
        // Validation
        if (string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(firstName))
        {
            await _menuService.DisplayMessageAsync("Required fields cannot be empty.", true);
            return false;
        }
        
        // Create and save data
        var newCab = new CabDetails
        {
            CarInfo = new CarInfo { ModelName = modelName },
            DriverInfo = new DriverInfo { FirstName = firstName, LastName = lastName },
            CurrentWorkState = WorkState.IDLE
        };
        
        await _dataStore.AddCabAsync(newCab);
        await _menuService.DisplayMessageAsync($"Cab added successfully with ID: {newCab.Id}", false);
        return true;
    }
    catch (Exception ex)
    {
        await _menuService.DisplayMessageAsync($"Error adding cab: {ex.Message}", true);
        return false;
    }
}
```

**ExitAction Execution Flow:**
```csharp
// User selects 'X' from main menu
// MenuOrchestrator finds ExitAction with Key = 'X'
// Calls: await exitAction.ExecuteAsync()

public override async Task<bool> ExecuteAsync()
{
    await _menuService.DisplayMessageAsync("Thank you for using Cab Management System. Goodbye!", false);
    await _menuService.WaitForUserAsync();
    
    // Signal the orchestrator to stop
    _isRunning = false;
    return true;
}
```

#### 5. Hierarchical Sub-Menu System

The system supports hierarchical navigation with sub-menus for better organization. Here's how to implement a multi-level menu structure:

```csharp
public class MenuOrchestrator : IMenuOrchestrator
{
    private readonly IMenuService _menuService;
    private readonly IDataStore _dataStore;
    private readonly IAppLogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private bool _isRunning = true;
    private Stack<string> _menuStack = new Stack<string>(); // Track navigation history
    
    // Main menu actions - these lead to sub-menus
    private List<IMenuAction> CreateMainMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<CabMenuAction>(),
            _serviceProvider.GetRequiredService<TripMenuAction>(),
            _serviceProvider.GetRequiredService<DriverMenuAction>(),
            _serviceProvider.GetRequiredService<ReportsMenuAction>(),
            _serviceProvider.GetRequiredService<ExitAction>()
        };
    }
    
    // Cab sub-menu actions
    private List<IMenuAction> CreateCabMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<ViewCabsAction>(),
            _serviceProvider.GetRequiredService<AddCabAction>(),
            _serviceProvider.GetRequiredService<EditCabAction>(),
            _serviceProvider.GetRequiredService<DeleteCabAction>(),
            _serviceProvider.GetRequiredService<SearchCabsAction>(),
            _serviceProvider.GetRequiredService<BackToMainMenuAction>()
        };
    }
    
    // Trip sub-menu actions
    private List<IMenuAction> CreateTripMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<ViewTripsAction>(),
            _serviceProvider.GetRequiredService<AddTripAction>(),
            _serviceProvider.GetRequiredService<EditTripAction>(),
            _serviceProvider.GetRequiredService<CompleteTripAction>(),
            _serviceProvider.GetRequiredService<CancelTripAction>(),
            _serviceProvider.GetRequiredService<BackToMainMenuAction>()
        };
    }
    
    // Driver sub-menu actions
    private List<IMenuAction> CreateDriverMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<ViewDriversAction>(),
            _serviceProvider.GetRequiredService<AddDriverAction>(),
            _serviceProvider.GetRequiredService<EditDriverAction>(),
            _serviceProvider.GetRequiredService<DeleteDriverAction>(),
            _serviceProvider.GetRequiredService<BackToMainMenuAction>()
        };
    }
    
    // Reports sub-menu actions
    private List<IMenuAction> CreateReportsMenuActions()
    {
        return new List<IMenuAction>
        {
            _serviceProvider.GetRequiredService<DailyReportAction>(),
            _serviceProvider.GetRequiredService<WeeklyReportAction>(),
            _serviceProvider.GetRequiredService<MonthlyReportAction>(),
            _serviceProvider.GetRequiredService<DriverPerformanceReportAction>(),
            _serviceProvider.GetRequiredService<BackToMainMenuAction>()
        };
    }
    
    // Navigation method that handles different menu levels
    public async Task ShowMenuAsync(string menuType = "main")
    {
        _menuStack.Push(menuType);
        
        while (_isRunning && _menuStack.Count > 0)
        {
            try
            {
                var currentMenu = _menuStack.Peek();
                var actions = GetActionsForMenu(currentMenu);
                
                // Display the current menu
                await _menuService.DisplayMenuAsync(GetMenuTitle(currentMenu), actions);
                
                // Get user input
                var userInput = await _menuService.GetUserInputAsync();
                
                // Process the user's choice
                await HandleUserInputAsync(userInput, actions, currentMenu);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in menu loop for {_menuStack.Peek()}", ex);
                await _menuService.DisplayMessageAsync("An error occurred. Please try again.", true);
                await _menuService.WaitForUserAsync();
            }
        }
    }
    
    private List<IMenuAction> GetActionsForMenu(string menuType)
    {
        return menuType switch
        {
            "main" => CreateMainMenuActions(),
            "cabs" => CreateCabMenuActions(),
            "trips" => CreateTripMenuActions(),
            "drivers" => CreateDriverMenuActions(),
            "reports" => CreateReportsMenuActions(),
            _ => CreateMainMenuActions()
        };
    }
    
    private string GetMenuTitle(string menuType)
    {
        return menuType switch
        {
            "main" => "Cab Management System - Main Menu",
            "cabs" => "Cab Management",
            "trips" => "Trip Management", 
            "drivers" => "Driver Management",
            "reports" => "Reports & Analytics",
            _ => "Cab Management System"
        };
    }
    
    // Enhanced input handling with navigation support
    public async Task HandleUserInputAsync(string input, List<IMenuAction> actions, string currentMenu)
    {
        if (string.IsNullOrEmpty(input))
        {
            await _menuService.DisplayMessageAsync("Please enter a valid choice.", true);
            return;
        }
        
        var choice = input.ToUpper().FirstOrDefault();
        
        // Find the action that matches the user's choice
        var selectedAction = actions.FirstOrDefault(a => a.Key == choice && a.IsEnabled);
        
        if (selectedAction == null)
        {
            await _menuService.DisplayMessageAsync("Invalid choice. Please try again.", true);
            return;
        }
        
        try
        {
            _logger.LogInfo($"Executing action: {selectedAction.Title} in menu: {currentMenu}");
            
            // Execute the selected action
            var success = await selectedAction.ExecuteAsync();
            
            if (success)
            {
                _logger.LogInfo($"Action completed successfully: {selectedAction.Title}");
            }
            else
            {
                _logger.LogWarning($"Action completed with issues: {selectedAction.Title}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error executing action: {selectedAction.Title}", ex);
            await _menuService.DisplayMessageAsync($"Error executing {selectedAction.Title}: {ex.Message}", true);
        }
        
        // Wait for user acknowledgment before returning to menu
        await _menuService.WaitForUserAsync();
    }
    
    // Navigation methods
    public void NavigateToMenu(string menuType)
    {
        _menuStack.Push(menuType);
    }
    
    public void NavigateBack()
    {
        if (_menuStack.Count > 1)
        {
            _menuStack.Pop();
        }
    }
    
    public void NavigateToMain()
    {
        _menuStack.Clear();
        _menuStack.Push("main");
    }
}
```

#### 6. Sub-Menu Action Implementations

**CabMenuAction - Entry point to cab operations:**
```csharp
public class CabMenuAction : IMenuAction
{
    private readonly IServiceProvider _serviceProvider;
    
    public CabMenuAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public string Title => "Cabs";
    public string Description => "Manage cabs - view, add, edit, delete";
    public char Key => 'C';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        // Navigate to cab sub-menu
        var orchestrator = _serviceProvider.GetRequiredService<IMenuOrchestrator>();
        orchestrator.NavigateToMenu("cabs");
        return true;
    }
}
```

**DriverMenuAction - Entry point to driver operations:**
```csharp
public class DriverMenuAction : IMenuAction
{
    private readonly IServiceProvider _serviceProvider;
    
    public DriverMenuAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public string Title => "Drivers";
    public string Description => "Manage drivers - view, add, edit, delete";
    public char Key => 'D';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        // Navigate to driver sub-menu
        var orchestrator = _serviceProvider.GetRequiredService<IMenuOrchestrator>();
        orchestrator.NavigateToMenu("drivers");
        return true;
    }
}
```

**LocationMenuAction - Entry point to location operations:**
```csharp
public class LocationMenuAction : IMenuAction
{
    private readonly IServiceProvider _serviceProvider;
    
    public LocationMenuAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public string Title => "Locations";
    public string Description => "Manage locations - view, add, edit, delete";
    public char Key => 'L';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        // Navigate to location sub-menu
        var orchestrator = _serviceProvider.GetRequiredService<IMenuOrchestrator>();
        orchestrator.NavigateToMenu("locations");
        return true;
    }
}
```

**TripMenuAction - Entry point to trip operations:**
```csharp
public class TripMenuAction : IMenuAction
{
    private readonly IServiceProvider _serviceProvider;
    
    public TripMenuAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public string Title => "Trips";
    public string Description => "Manage trips - view, add, edit, complete";
    public char Key => 'T';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        // Navigate to trip sub-menu
        var orchestrator = _serviceProvider.GetRequiredService<IMenuOrchestrator>();
        orchestrator.NavigateToMenu("trips");
        return true;
    }
}
```

**ReportsMenuAction - Entry point to reports:**
```csharp
public class ReportsMenuAction : IMenuAction
{
    private readonly IServiceProvider _serviceProvider;
    
    public ReportsMenuAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public string Title => "Reports";
    public string Description => "View reports and analytics";
    public char Key => 'R';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        // Navigate to reports sub-menu
        var orchestrator = _serviceProvider.GetRequiredService<IMenuOrchestrator>();
        orchestrator.NavigateToMenu("reports");
        return true;
    }
}
```

**BackToMainMenuAction - Navigation back to main menu:**
```csharp
public class BackToMainMenuAction : IMenuAction
{
    private readonly IServiceProvider _serviceProvider;
    
    public BackToMainMenuAction(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public string Title => "Back to Main Menu";
    public string Description => "Return to the main menu";
    public char Key => 'B';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        // Navigate back to main menu
        var orchestrator = _serviceProvider.GetRequiredService<IMenuOrchestrator>();
        orchestrator.NavigateToMain();
        return true;
    }
}
```

**Direct IMenuAction Implementation Pattern:**
All menu actions directly implement `IMenuAction` interface without inheritance. Each action has its own constructor with only the dependencies it needs.

**Example Pattern:**
```csharp
public class SomeMenuAction : IMenuAction
{
    private readonly IDataStore _dataStore;
    private readonly IMenuService _menuService;
    private readonly IAppLogger _logger;
    
    public SomeMenuAction(IDataStore dataStore, IMenuService menuService, IAppLogger logger)
    {
        _dataStore = dataStore;
        _menuService = menuService;
        _logger = logger;
    }
    
    public string Title => "Action Title";
    public string Description => "Action Description";
    public char Key => 'K';
    public bool IsEnabled => true;
    
    public async Task<bool> ExecuteAsync()
    {
        // Implementation here
        return true;
    }
}
```

#### 7. Menu Configuration for Sub-Menus

```csharp
public static class MenuConfiguration
{
    public static class MainMenuKeys
    {
        public const char CABS = 'C';
        public const char DRIVERS = 'D';
        public const char LOCATIONS = 'L';
        public const char TRIPS = 'T';
        public const char REPORTS = 'R';
        public const char EXIT = 'X';
    }
    
    public static class CabMenuKeys
    {
        public const char VIEW_CABS = 'V';
        public const char ADD_CAB = 'A';
        public const char EDIT_CAB = 'E';
        public const char DELETE_CAB = 'D';
        public const char SEARCH_CABS = 'S';
        public const char BACK_TO_MAIN = 'B';
    }
    
    public static class DriverMenuKeys
    {
        public const char VIEW_DRIVERS = 'V';
        public const char ADD_DRIVER = 'A';
        public const char EDIT_DRIVER = 'E';
        public const char DELETE_DRIVER = 'D';
        public const char SEARCH_DRIVERS = 'S';
        public const char BACK_TO_MAIN = 'B';
    }
    
    public static class LocationMenuKeys
    {
        public const char VIEW_LOCATIONS = 'V';
        public const char ADD_LOCATION = 'A';
        public const char EDIT_LOCATION = 'E';
        public const char DELETE_LOCATION = 'D';
        public const char SEARCH_LOCATIONS = 'S';
        public const char BACK_TO_MAIN = 'B';
    }
    
    public static class TripMenuKeys
    {
        public const char VIEW_TRIPS = 'V';
        public const char ADD_TRIP = 'A';
        public const char EDIT_TRIP = 'E';
        public const char COMPLETE_TRIP = 'C';
        public const char CANCEL_TRIP = 'X';
        public const char SEARCH_TRIPS = 'S';
        public const char BACK_TO_MAIN = 'B';
    }
    
    public static class ReportsMenuKeys
    {
        public const char DAILY_REPORT = 'D';
        public const char WEEKLY_REPORT = 'W';
        public const char MONTHLY_REPORT = 'M';
        public const char DRIVER_PERFORMANCE = 'P';
        public const char CAB_UTILIZATION = 'U';
        public const char BACK_TO_MAIN = 'B';
    }
}
```

#### 8. Updated Dependency Injection Configuration

```csharp
.ConfigureServices((hostContext, services) =>
{
    // Core Services Registration
    services.AddSingleton<IAppLogger, AppLogger>();
    services.AddSingleton<IDataStore, DataStore>(provider =>
    {
        var logger = provider.GetService<IAppLogger>();
        var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        return new DataStore(dataDirectory, logger!);
    });

    // Menu Services Registration
    services.AddSingleton<IMenuService, MenuService>();
    services.AddSingleton<IMenuOrchestrator, MenuOrchestrator>();

    // Main Menu Actions (Entity Navigation)
    services.AddTransient<CabMenuAction>();
    services.AddTransient<DriverMenuAction>();
    services.AddTransient<LocationMenuAction>();
    services.AddTransient<TripMenuAction>();
    services.AddTransient<ReportsMenuAction>();
    services.AddTransient<ExitAction>();

    // Cab Sub-Menu Actions
    services.AddTransient<ViewCabsAction>();
    services.AddTransient<AddCabAction>();
    services.AddTransient<EditCabAction>();
    services.AddTransient<DeleteCabAction>();
    services.AddTransient<SearchCabsAction>();

    // Driver Sub-Menu Actions
    services.AddTransient<ViewDriversAction>();
    services.AddTransient<AddDriverAction>();
    services.AddTransient<EditDriverAction>();
    services.AddTransient<DeleteDriverAction>();
    services.AddTransient<SearchDriversAction>();

    // Location Sub-Menu Actions
    services.AddTransient<ViewLocationsAction>();
    services.AddTransient<AddLocationAction>();
    services.AddTransient<EditLocationAction>();
    services.AddTransient<DeleteLocationAction>();
    services.AddTransient<SearchLocationsAction>();

    // Trip Sub-Menu Actions
    services.AddTransient<ViewTripsAction>();
    services.AddTransient<AddTripAction>();
    services.AddTransient<EditTripAction>();
    services.AddTransient<CompleteTripAction>();
    services.AddTransient<CancelTripAction>();
    services.AddTransient<SearchTripsAction>();

    // Reports Sub-Menu Actions
    services.AddTransient<DailyReportAction>();
    services.AddTransient<WeeklyReportAction>();
    services.AddTransient<MonthlyReportAction>();
    services.AddTransient<DriverPerformanceReportAction>();
    services.AddTransient<CabUtilizationReportAction>();

    // Navigation Actions
    services.AddTransient<BackToMainMenuAction>();

    // Application Registration
    services.AddSingleton<App>();
});
```

#### 9. Example Menu Flow

**Main Menu Display (Entity-Based):**
```
============================================================
  CAB MANAGEMENT SYSTEM - MAIN MENU
============================================================
  C. Cabs
     Manage cabs - view, add, edit, delete
  D. Drivers
     Manage drivers - view, add, edit, delete
  L. Locations
     Manage locations - view, add, edit, delete
  T. Trips
     Manage trips - view, add, edit, complete
  R. Reports
     View reports and analytics
  X. Exit
     Exit the application
```

**Cab Sub-Menu Display (Actions for Cabs):**
```
============================================================
  CAB MANAGEMENT
============================================================
  V. View All Cabs
     Display all cabs in the system with their details
  A. Add New Cab
     Add a new cab to the system
  E. Edit Cab
     Modify existing cab information
  D. Delete Cab
     Remove a cab from the system
  S. Search Cabs
     Find cabs by model or driver name
  B. Back to Main Menu
     Return to the main menu
```

**Driver Sub-Menu Display (Actions for Drivers):**
```
============================================================
  DRIVER MANAGEMENT
============================================================
  V. View All Drivers
     Display all drivers in the system
  A. Add New Driver
     Add a new driver to the system
  E. Edit Driver
     Modify existing driver information
  D. Delete Driver
     Remove a driver from the system
  S. Search Drivers
     Find drivers by name or license
  B. Back to Main Menu
     Return to the main menu
```

**Location Sub-Menu Display (Actions for Locations):**
```
============================================================
  LOCATION MANAGEMENT
============================================================
  V. View All Locations
     Display all locations in the system
  A. Add New Location
     Add a new location to the system
  E. Edit Location
     Modify existing location information
  D. Delete Location
     Remove a location from the system
  S. Search Locations
     Find locations by name or address
  B. Back to Main Menu
     Return to the main menu
```

**Trip Sub-Menu Display (Actions for Trips):**
```
============================================================
  TRIP MANAGEMENT
============================================================
  V. View All Trips
     Display all trips in the system
  A. Add New Trip
     Create a new trip
  E. Edit Trip
     Modify existing trip information
  C. Complete Trip
     Mark a trip as completed
  X. Cancel Trip
     Cancel an active trip
  S. Search Trips
     Find trips by criteria
  B. Back to Main Menu
     Return to the main menu
```

**Reports Sub-Menu Display (Actions for Reports):**
```
============================================================
  REPORTS & ANALYTICS
============================================================
  D. Daily Report
     Generate daily activity report
  W. Weekly Report
     Generate weekly summary report
  M. Monthly Report
     Generate monthly analytics report
  P. Driver Performance
     View driver performance metrics
  U. Cab Utilization
     View cab utilization statistics
  B. Back to Main Menu
     Return to the main menu
```

#### 10. Benefits of Hierarchical Menu System

1. **Better Organization**: Related operations are grouped together logically
2. **Reduced Cognitive Load**: Users see fewer options at once, making decisions easier
3. **Scalability**: Easy to add new sub-menus and actions without cluttering the main menu
4. **Navigation History**: Stack-based navigation allows for proper back navigation
5. **Context Awareness**: Each sub-menu can have context-specific actions and validations
6. **Consistent UX**: Uniform navigation patterns across all sub-menus
7. **Extensibility**: New menu levels can be added easily (e.g., sub-sub-menus)

#### 6. Error Handling and Recovery

```csharp
public async Task HandleUserInputAsync(string input, List<IMenuAction> actions)
{
    // ... input validation ...
    
    try
    {
        var success = await selectedAction.ExecuteAsync();
        
        // Handle action-specific results
        if (!success)
        {
            await _menuService.DisplayMessageAsync(
                $"{selectedAction.Title} completed with issues. Please check the logs.", true);
        }
    }
    catch (ValidationException ex)
    {
        // Handle validation errors specifically
        await _menuService.DisplayMessageAsync($"Validation Error: {ex.Message}", true);
    }
    catch (DataAccessException ex)
    {
        // Handle data access errors
        await _menuService.DisplayMessageAsync("Data access error. Please try again.", true);
        _logger.LogError("Data access error in action", ex);
    }
    catch (Exception ex)
    {
        // Handle unexpected errors
        await _menuService.DisplayMessageAsync("An unexpected error occurred.", true);
        _logger.LogError($"Unexpected error in {selectedAction.Title}", ex);
    }
}
```

### Key Benefits of This Execution Flow

1. **Centralized Control**: All action execution goes through the orchestrator, providing consistent error handling and logging.

2. **Dependency Injection**: Actions are created fresh for each menu display, ensuring clean state and proper dependency resolution.

3. **Polymorphic Execution**: The orchestrator doesn't need to know about specific action implementations - it works with the `IMenuAction` interface.

4. **Error Isolation**: Errors in one action don't crash the entire application - they're caught and handled gracefully.

5. **Extensibility**: New actions can be added by simply registering them in the DI container and including them in the action list.

6. **State Management**: The orchestrator maintains the application state (`_isRunning`) and coordinates between different actions.

</details>

<details>
<summary><strong>🔗 Application Integration</strong></summary>

### Updated App Class

```csharp
public class App
{
    private readonly IAppLogger _appLogger;
    private readonly IMenuOrchestrator _menuOrchestrator;

    public App(IAppLogger appLogger, IMenuOrchestrator menuOrchestrator)
    {
        _appLogger = appLogger;
        _menuOrchestrator = menuOrchestrator;
    }

    public async Task Run(string[] args)
    {
        try
        {
            _appLogger.LogInfo("Application Started");
            
            // Start with the main menu - the orchestrator handles all navigation
            await _menuOrchestrator.ShowMenuAsync("main");
        }
        catch(Exception ex)
        {
            _appLogger.LogError("An error occurred", ex);
        }
        finally
        {
            _appLogger.LogInfo("Application Ended");
        }
    }
}
```

### Complete Navigation Flow Example

Here's how the complete navigation flow works in practice:

**1. Application Start:**
```
App.Run() → MenuOrchestrator.ShowMenuAsync("main") → Display Main Menu
```

**2. User selects 'C' (Cab Management):**
```
Main Menu → CabMenuAction.ExecuteAsync() → NavigateToMenu("cabs") → Display Cab Sub-Menu
```

**3. User selects 'V' (View Cabs):**
```
Cab Sub-Menu → ViewCabsAction.ExecuteAsync() → Display cab data → Return to Cab Sub-Menu
```

**4. User selects 'B' (Back to Main):**
```
Cab Sub-Menu → BackToMainMenuAction.ExecuteAsync() → NavigateToMain() → Display Main Menu
```

**5. User selects 'X' (Exit):**
```
Main Menu → ExitAction.ExecuteAsync() → Set _isRunning = false → Application ends
```

### Menu State Management

The `MenuOrchestrator` maintains navigation state through:

```csharp
public class MenuOrchestrator : IMenuOrchestrator
{
    private Stack<string> _menuStack = new Stack<string>();
    
    // Navigation methods maintain the stack
    public void NavigateToMenu(string menuType)
    {
        _menuStack.Push(menuType);
        // This will cause the next iteration of ShowMenuAsync to display the new menu
    }
    
    public void NavigateBack()
    {
        if (_menuStack.Count > 1)
        {
            _menuStack.Pop();
        }
    }
    
    public void NavigateToMain()
    {
        _menuStack.Clear();
        _menuStack.Push("main");
    }
    
    // The main loop checks the stack to determine which menu to show
    public async Task ShowMenuAsync(string menuType = "main")
    {
        _menuStack.Push(menuType);
        
        while (_isRunning && _menuStack.Count > 0)
        {
            var currentMenu = _menuStack.Peek();
            var actions = GetActionsForMenu(currentMenu);
            
            await _menuService.DisplayMenuAsync(GetMenuTitle(currentMenu), actions);
            var userInput = await _menuService.GetUserInputAsync();
            await HandleUserInputAsync(userInput, actions, currentMenu);
        }
    }
}
```

**Design Rationale:**
- **Async Integration**: App.Run is now async to support menu orchestration
- **Dependency Injection**: MenuOrchestrator injected through constructor
- **Error Handling**: Comprehensive error handling with logging
- **Lifecycle Management**: Clear start/end logging for application lifecycle

</details>

<details>
<summary><strong>⚙️ Configuration and Extensibility</strong></summary>

### MenuConfiguration Class

```csharp
public class MenuConfiguration
{
    public static class MenuKeys
    {
        public const char VIEW_CABS = 'V';
        public const char ADD_CAB = 'A';
        public const char VIEW_TRIPS = 'T';
        public const char ADD_TRIP = 'N';
        public const char VIEW_DRIVERS = 'D';
        public const char EXIT = 'X';
    }
    
    public static class ValidationRules
    {
        public static bool ValidateCabInput(CabDetails cab)
        {
            return !string.IsNullOrEmpty(cab.CarInfo?.ModelName) &&
                   !string.IsNullOrEmpty(cab.DriverInfo?.FirstName);
        }
    }
}
```

**Design Rationale:**
- **Centralized Configuration**: All menu constants in one place
- **Type Safety**: Compile-time checking of menu keys
- **Validation Logic**: Reusable validation rules
- **Maintainability**: Easy to modify menu structure

</details>

<details>
<summary><strong>✨ Benefits of This Design</strong></summary>

### 1. **Maintainability**
- Clear separation of concerns
- Consistent patterns across all components
- Easy to locate and modify specific functionality

### 2. **Testability**
- Interface-based design enables easy mocking
- Each component can be tested in isolation
- Clear dependencies make testing straightforward

### 3. **Extensibility**
- Easy to add new menu actions
- Simple to create sub-menus and complex navigation
- Configuration-driven approach for customization

### 4. **Performance**
- Async/await pattern for non-blocking operations
- Efficient data access through existing DataStore
- Minimal memory footprint with proper lifetime management

### 5. **User Experience**
- Consistent and intuitive interface
- Clear error messages and feedback
- Responsive navigation and input handling

### 6. **Integration**
- Seamless integration with existing infrastructure
- Leverages existing logging and data storage
- No breaking changes to existing code

</details>

<details>
<summary><strong>🤔 Implementation Considerations</strong></summary>

### 1. **Error Handling Strategy**
- All menu actions include comprehensive error handling
- User-friendly error messages
- Detailed logging for debugging
- Graceful degradation on errors

### 2. **Data Validation**
- Client-side validation for immediate feedback
- Server-side validation for data integrity
- Consistent validation patterns across all actions

### 3. **Performance Optimization**
- Async operations for better responsiveness
- Efficient data access patterns
- Minimal object creation and disposal

### 4. **Security Considerations**
- Input sanitization for user data
- Proper error message handling (no sensitive data exposure)
- Secure data storage through existing DataStore

</details>

<details>
<summary><strong>🚀 Future Enhancements</strong></summary>

### 1. **Advanced Features**
- Menu history and navigation
- User preferences and customization
- Advanced search and filtering
- Export/import functionality

### 2. **UI Improvements**
- Color coding for different message types
- Progress indicators for long operations
- Interactive data editing
- Form validation with real-time feedback

### 3. **Integration Options**
- Web-based menu interface
- API endpoints for external integration
- Database backend options
- Real-time data synchronization

This design provides a solid foundation for building a comprehensive menu system that is maintainable, extensible, and integrates seamlessly with the existing CabApp architecture.

</details>
