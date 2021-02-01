using Vim.Desktop.Api;
using Vim.Explorer.Plugin;

namespace Vim.Desktop.Plugin.Samples
{
    [VimPlugin]
    public class MyFirstPlugin : VimPluginBaseClassWithMouseHandling
    {
        public SliderListView SliderListView = new SliderListView();
        
        public override void OnOpenFile(string fileName)
        {
            var vim = VimScene.LoadVim(fileName);
            SliderListView.Init(new VimHelper(RenderApi, vim));
        }

        public override void OnFrameUpdate(float deltaTime)
        {
        }
    }
}
