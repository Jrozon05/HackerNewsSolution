output "angular_app_url" {
  value = azurerm_windows_web_app.angular_app.default_hostname
}

output "api_service_url" {
  value = azurerm_windows_web_app.api_service.default_hostname
}
