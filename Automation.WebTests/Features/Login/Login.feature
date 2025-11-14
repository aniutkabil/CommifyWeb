@ui
Feature: User Login
  As a carrier provider
  I want a username/password login on the tracking website
  So that only genuine customers can access their tracking data

  Background:
    Given valid user credentials are already registered

    @ui
  Scenario: Login screen is shown when user is not logged in
    Given I’m not logged in with a genuine user
    When I navigate to any page on the tracking site
    Then I am presented with a login screen

    @ui
  Scenario: Successful login with valid credentials
    Given I’m on the login screen
    When I enter a valid username and password and submit
    Then I am logged in successfully
