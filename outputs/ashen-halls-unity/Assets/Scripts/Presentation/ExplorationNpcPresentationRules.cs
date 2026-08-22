namespace AshenHalls
{
    public static class ExplorationNpcPresentationRules
    {
        public static float NamedObjectPadding(bool wideView)
        {
            return wideView ? 0.14f : 0.05f;
        }

        public static float NamedArtPadding()
        {
            return 0.02f;
        }

        public static float NamedArtScale(bool wideView)
        {
            return wideView ? 1.02f : 1.08f;
        }

        public static bool ShouldDrawExteriorAmbientCitizen(bool wideView)
        {
            // Region view uses landmark and role markers; full-body passersby
            // become visual noise at that scale.
            return !wideView;
        }

        public static float ExteriorAmbientPadding(bool wideView)
        {
            // Ambient citizens remain slightly subordinate to interactive
            // actors through opacity, not a visibly different body scale.
            return wideView ? 0.16f : 0.06f;
        }

        public static float ExteriorAmbientAlpha(bool wideView, bool yieldingToParty)
        {
            if (yieldingToParty) return wideView ? 0.54f : 0.68f;
            return wideView ? 0.64f : 0.82f;
        }

        public static float GrandHearthPatronPadding(bool wideView)
        {
            return wideView ? 0.15f : 0.06f;
        }

        public static float GrandHearthPatronAlpha(bool wideView)
        {
            return wideView ? 0.72f : 0.94f;
        }
    }
}
