namespace Entities.MasterData;

public class MenuItem
{
    public string MenuName { get; set; } = "";
    public string ControllerName { get; set; } = "";
    public string MenuParentId { get; set; } = "";
    public string Nature { get; set; } = "";
    public string Icon { get; set; } = "";
    public double Rank { get; set; }
    public string MenuId { get; set; } = "";
    public bool ViewOnly { get; set; }
    public bool Visible { get; set; }
    public List<MenuItem> SubMenus { get; set; } = [];
}
