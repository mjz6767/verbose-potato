using System;
using System.Collections.Generic;

namespace AshenHalls
{
    public static class InventoryItemIdentityRules
    {
        public const int SchemaVersion = 27;
        private const string NewItemPrefix = "item-";
        private const string LegacyItemPrefix = "legacy-item-";

        public static int NormalizeInstanceIds(IList<InventoryItem> inventory, int campaignSeed)
        {
            if (inventory == null) return 0;

            HashSet<string> reserved = new HashSet<string>(StringComparer.Ordinal);
            List<int> repairIndices = new List<int>();
            int repaired = 0;

            for (int i = 0; i < inventory.Count; i++)
            {
                InventoryItem item = inventory[i];
                if (item == null) continue;
                string id = NormalizeId(item.InstanceId);
                if (id.Length > 0 && reserved.Add(id))
                {
                    if (!string.Equals(item.InstanceId, id, StringComparison.Ordinal))
                    {
                        item.InstanceId = id;
                        repaired++;
                    }
                    continue;
                }

                repairIndices.Add(i);
            }

            foreach (int index in repairIndices)
            {
                InventoryItem item = inventory[index];
                string stem = LegacyItemId(campaignSeed, index);
                string candidate = stem;
                int suffix = 2;
                while (!reserved.Add(candidate)) candidate = stem + "-" + suffix++;
                item.InstanceId = candidate;
                repaired++;
            }

            return repaired;
        }

        public static string[] DuplicateInstanceIds(IEnumerable<InventoryItem> inventory)
        {
            if (inventory == null) return Array.Empty<string>();

            HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);
            HashSet<string> duplicates = new HashSet<string>(StringComparer.Ordinal);
            foreach (InventoryItem item in inventory)
            {
                if (item == null) continue;
                string id = NormalizeId(item.InstanceId);
                if (id.Length > 0 && !seen.Add(id)) duplicates.Add(id);
            }

            string[] result = new string[duplicates.Count];
            duplicates.CopyTo(result);
            Array.Sort(result, StringComparer.Ordinal);
            return result;
        }

        public static string EnsureAdmissionId(InventoryItem item, IEnumerable<InventoryItem> existingInventory)
        {
            if (item == null) return "";

            HashSet<string> reserved = new HashSet<string>(StringComparer.Ordinal);
            if (existingInventory != null)
            {
                foreach (InventoryItem existing in existingInventory)
                {
                    if (existing == null || ReferenceEquals(existing, item)) continue;
                    string existingId = NormalizeId(existing.InstanceId);
                    if (existingId.Length > 0) reserved.Add(existingId);
                }
            }

            string id = NormalizeId(item.InstanceId);
            if (id.Length > 0 && !reserved.Contains(id))
            {
                item.InstanceId = id;
                return id;
            }

            do
            {
                id = NewItemPrefix + Guid.NewGuid().ToString("N");
            }
            while (reserved.Contains(id));

            item.InstanceId = id;
            return id;
        }

        public static InventoryItem FindById(IEnumerable<InventoryItem> inventory, string instanceId)
        {
            string id = NormalizeId(instanceId);
            if (id.Length == 0 || inventory == null) return null;
            foreach (InventoryItem item in inventory)
            {
                if (item != null && string.Equals(item.InstanceId, id, StringComparison.Ordinal)) return item;
            }
            return null;
        }

        public static bool HasUniqueInstanceIds(IEnumerable<InventoryItem> inventory)
        {
            if (inventory == null) return true;
            HashSet<string> ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (InventoryItem item in inventory)
            {
                if (item == null) continue;
                string id = NormalizeId(item.InstanceId);
                if (id.Length == 0 || !ids.Add(id)) return false;
            }
            return true;
        }

        private static string NormalizeId(string instanceId)
        {
            return (instanceId ?? "").Trim();
        }

        private static string LegacyItemId(int campaignSeed, int index)
        {
            return LegacyItemPrefix
                + unchecked((uint)campaignSeed).ToString("x8")
                + "-"
                + (index + 1).ToString("x4");
        }
    }
}
