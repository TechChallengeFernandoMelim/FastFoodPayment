Feature: CreatePayment
    As a restaurant customer
    I want to be able to create a payment QR code
    So that customers can pay for their orders

Scenario: Successfully create a payment QR code
    Given a valid payment request with description "teste", external reference "teste", notification URL "teste", title "teste", total amount 12, and items with title "teste", quantity 1, total amount 5, unit measure "u", and unit price 5
    And the Mercado Pago environment variables are set
    When a customer attempts to create a payment
    Then the system should return a QR code data and in-store order ID
    And the payment request details should match the original request