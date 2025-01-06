variable "location" {
  default = "East US"
}

variable "resource_group_name" {
  default = "hacker-news-rg"
}

resource "azurerm_resource_group" "rg" {
  name     = "hacker-news-resource-group"
  location = "East US"
}
