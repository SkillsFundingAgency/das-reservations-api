Feature: DeleteReservation
	In order to remove a reservation
	As a employer or provider
	I would like to delete a reservation

Scenario: Delete a pending reservation as a non levy employer
	Given I have a non levy account
	And I have an existing reservation with status Pending
	When I delete the reservation
	Then the reservation should be deleted

Scenario: Delete a confirm reservation as a non levy employer
	Given I have a non levy account
	And I have an existing reservation with status Confirmed
	When I delete the reservation
	Then the reservation should not be deleted

Scenario: Try to delete an completed reservation as a non levy employer
	Given I have a non levy account
	And I have an existing reservation with status Completed
	When I delete the reservation
	Then the reservation should not be deleted

Scenario: Try to delete an already deleted reservation as a non levy employer
	Given I have a non levy account
	And I have an existing reservation with status Deleted
	When I delete the reservation
	Then the reservation should be deleted

