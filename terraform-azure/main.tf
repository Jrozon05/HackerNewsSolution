provider "azurerm" {
  features {}
}

resource "azurerm_service_plan" "asp" {
  name                = "my-app-service-plan"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Windows"
  sku_name            = "S1" # Change SKU as needed (e.g., "S1" for Standard)
}

resource "azurerm_windows_web_app" "angular_app" {
  name                = "hacker-news-angular"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.asp.id

  site_config {
    app_command_line = "node start.js"
  }

  app_settings = {
    WEBSITE_NODE_DEFAULT_VERSION = "~16"
    WEBSITE_RUN_FROM_PACKAGE     = "1"
  }
}

resource "azurerm_windows_web_app" "api_service" {
  name                = "hacker-news-api"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.asp.id

  site_config {
    app_command_line = "dotnet run"
  }

  app_settings = {
    WEBSITE_RUN_FROM_PACKAGE = "1"
  }
}
