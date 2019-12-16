Feature: Get Account Legal Entities
	In order to associated a reservation with a legal entity
	As a employer or provider
	I want to be able to retrieve all legal entitiies associated with my account

Scenario: Get available legal entities for non levy account
	Given I have a non levy account 
	And I have a legal entity with unsigned agreement
	And I have a legal entity with signed agreement
	When I get my legal entities attached to the account
	Then all legal entities for my account are returned

Scenario: Get available legal entities for levy account
	Given I have a levy account 
	And I have a legal entity with unsigned agreement
	And I have a legal entity with signed agreement
	When I get my legal entities attached to the account
	Then all legal entities for my account are returned