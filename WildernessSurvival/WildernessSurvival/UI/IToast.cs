namespace WildernessSurvival.UI {
    public interface IToast {
        void LongAlert(string message);
        void ShortAlert(string message);

        void Clear();
    }
}
