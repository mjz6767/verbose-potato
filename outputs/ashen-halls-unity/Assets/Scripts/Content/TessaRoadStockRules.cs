using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public sealed class TessaRoadStockOffer
    {
        public readonly string SignatureId;
        public readonly string ReceiptFlag;
        public readonly int Price;
        public TessaRoadStockOffer(string signatureId, string receiptFlag, int price)
        { SignatureId = signatureId; ReceiptFlag = receiptFlag; Price = price; }
    }

    public static class TessaRoadStockRules
    {
        private static readonly TessaRoadStockOffer[] Offers =
        {
            new TessaRoadStockOffer("bridgehook_spear", "tessa_bridgehook_spear_bought", 76),
            new TessaRoadStockOffer("stonecleft_axe", "tessa_stonecleft_axe_bought", 76)
        };
        public static IReadOnlyList<TessaRoadStockOffer> All => Offers;
        public static TessaRoadStockOffer Find(string id) => Array.Find(Offers, offer => offer.SignatureId == id);
        public static bool Available(IReadOnlyCollection<string> flags) => Has(flags, StoryFlags.KoboldKingDefeated);
        public static bool Purchased(IReadOnlyCollection<string> flags, TessaRoadStockOffer offer) => offer != null && Has(flags, offer.ReceiptFlag);
        private static bool Has(IReadOnlyCollection<string> flags, string flag)
        {
            if (flags == null) return false;
            foreach (string value in flags) if (value == flag) return true;
            return false;
        }
    }
}
