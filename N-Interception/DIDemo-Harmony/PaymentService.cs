// ============================================================
//  IPaymentService + PaymentService
//
//  Die Klasse bleibt komplett unverändert.
//  Castle.DynamicProxy generiert zur Laufzeit eine neue Subklasse,
//  die jeden Aufruf an die Interceptors weiterleitet –
//  kein einziger Patch in diesem File.
// ============================================================

public interface IPaymentService
{
    bool ProcessPayment(string orderId, decimal amount);
    void Refund(string orderId, decimal amount);
}

public class PaymentService : IPaymentService
{
    public bool ProcessPayment(string orderId, decimal amount)
    {
        Console.WriteLine($"  [PaymentService] Verarbeite Zahlung: Order={orderId}, Betrag={amount:F2} €");
        return amount > 0;
    }

    public void Refund(string orderId, decimal amount)
    {
        Console.WriteLine($"  [PaymentService] Rückerstattung: Order={orderId}, Betrag={amount:F2} €");
    }
}
