namespace CrowdControl.Games.Packs.MCCCursedHaloCE;

/// <summary>
/// Crappy TOML parser that only supports kvps. No tables. No arrays. No numbers. Just strings as KVP.
/// </summary>
public class BadToml {
    private Dictionary<string, string> _kvp = new();
    public string this[string key] => _kvp[key];

    public void Add(string key, string value) => _kvp[key] = value;
    public static BadToml Parse(string tomlContent) {
        BadToml tomlTable = new BadToml();
        
        string[] lines = tomlContent.Split('\n');

        foreach (string line in lines) {
            int index = line.IndexOf("#", StringComparison.Ordinal);

            string trimmed = line.Substring(0, index);
            if (string.IsNullOrWhiteSpace(trimmed)) {
                continue;
            }
            
            string[] columns = trimmed.Split('=');
            string key = columns[0].Trim();
            string value = columns[1].Trim();
            
            tomlTable.Add(key, value);
        }
        
        return tomlTable;
    }
}