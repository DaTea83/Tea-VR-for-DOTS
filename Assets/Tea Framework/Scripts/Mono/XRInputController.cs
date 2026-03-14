namespace TeaFramework {
    public sealed class XRInputController : GenericSingleton<XRInputController> {
        public XRDefault Input { get; private set; }

        private void OnEnable() {
            Input = new XRDefault();
            Input.Enable();
        }

        private void OnDisable() { Input.Disable(); }
    }
}