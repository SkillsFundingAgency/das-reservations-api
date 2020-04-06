Feature: Change of party
	In order to be able change employer or provider during an apprenticeship
	as a levy or a non levy employer
	I want my reservation to be carried over

@ignore
Scenario: Change employer, non-levy to non-levy
	Given I have the following existing reservation:
	| Account Legal Entity Id | Provider Id | Start Date | Created Date | Course Id | Status    | Is Levy Account |
	| 1                       | 15214       | 2019-07-01 | 2019-01-01   | 1         | Confirmed | False           |
	And I want to change account legal entity to 2
	When I call change of circumstances
	Then an http status code of 200 is returned
	And I have the following reservations:
	| Account Legal Entity Id | Provider Id | Start Date | Created Date | Course Id | Status    | Is Levy Account |
	| 1                       | 15214       | 2019-07-01 | 2019-01-01   | 1         | Confirmed | False           |
	| 2                       | 15214       | 2019-07-01 | today        | 1         | Change    | False           |

@ignore
Scenario: Change employer, levy to levy
	Given I have the following existing reservation:
	| Account Legal Entity Id | Provider Id | Start Date | Created Date | Course Id | Status    | Is Levy Account |
	| 10                      | 15214       | 2019-07-01 | 2019-01-01   | 1         | Confirmed | False           |
	And I want to change account legal entity to 20
	When I call change of circumstances
	Then an http status code of 200 is returned
	And I have the following reservations:
	| Account Legal Entity Id | Provider Id | Start Date | Created Date | Course Id | Status    | Is Levy Account |
	| 10                      | 15214       | 2019-07-01 | 2019-01-01   | 1         | Confirmed | False           |
	| 20                      | 15214       | 2019-07-01 | today        | 1         | Change    | False           |
	
@ignore
Scenario: Change employer, non-levy to levy
	Given I have the following existing reservation:
	| Account Legal Entity Id | Provider Id | Start Date | Created Date | Course Id | Status    | Is Levy Account |
	| 1                       | 15214       | 2019-07-01 | 2019-01-01   | 1         | Confirmed | False           |
	And I want to change account legal entity to 20
	When I call change of circumstances
	Then an http status code of 200 is returned
	And I have the following reservations:
	| Account Legal Entity Id | Provider Id | Start Date | Created Date | Course Id | Status    | Is Levy Account |
	| 1                       | 15214       | 2019-07-01 | 2019-01-01   | 1         | Confirmed | False           |
	| 20                      | 15214       | 2019-07-01 | today        | 1         | Change    | True            |

Scenario: Change employer, levy to non-levy - not supported
	Given I have the following existing reservation:
	| Account Legal Entity Id | Provider Id | Start Date | Created Date | Course Id | Status    | Is Levy Account |
	| 10                      | 15214       | 2019-07-01 | 2019-01-01   | 1         | Confirmed | True            |
	And I want to change account legal entity to 2
	When I call change of circumstances
	Then an http status code of 400 is returned
	
Scenario: Change provider
	Given I have the following existing reservation:
	| Account Legal Entity Id | Provider Id | Start Date | Created Date | Course Id | Status    | Is Levy Account |
	| 1                       | 15214       | 2019-07-01 | 2019-01-01   | 1         | Confirmed | False           |
	And I want to change provider to 45665
	When I call change of circumstances
	Then an http status code of 200 is returned
	And I have the following reservations:
	| Account Legal Entity Id | Provider Id | Start Date | Created Date | Course Id | Status    | Is Levy Account |
	| 1                       | 15214       | 2019-07-01 | 2019-01-01   | 1         | Confirmed | False           |
	| 1                       | 45665       | 2019-07-01 | today        | 1         | Change    | False           |

Scenario: Reservation deleted
	Given I have the following existing reservation:
	| Account Legal Entity Id | Provider Id | Start Date | Created Date | Course Id | Status  | Is Levy Account |
	| 1                       | 15214       | 2019-07-01 | 2019-01-01   | 1         | Deleted | False           |
	And I want to change account legal entity to 2
	When I call change of circumstances
	Then an http status code of 400 is returned


#todo
Scenario: Total reservation count not affected

Scenario: change reservations not returned in call to all
