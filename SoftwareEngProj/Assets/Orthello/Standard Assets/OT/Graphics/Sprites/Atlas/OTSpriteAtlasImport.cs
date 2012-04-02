using UnityEngine;
using System.Collections;
using System.Xml;

/// <summary>
/// Base class for importing sprite atlasses
/// </summary>
public class OTSpriteAtlasImport : OTSpriteAtlas
{
    
    /// <exclude />
    public TextAsset _atlasDataFile = null;
    /// <summary>
    /// Will reload the atlas data
    /// </summary>
    public bool reloadData = false;

    /// <summary>
    /// Atlas data file to import framedata from
    /// </summary>
    public TextAsset atlasDataFile
    {
        get
        {
            return _atlasDataFile;
        }
        set
        {
            _atlasDataFile = value;
            Update();
        }
    }

    private TextAsset _atlasDataFile_ = null;

    /// <exclude />
    new protected void Start()
    {
        _atlasDataFile_ = atlasDataFile;
        base.Start();
    }


    /// <summary>
    /// Override this Import method to load the atlas data from the xml
    /// </summary>
    /// <returns>Array with atlas frame data</returns>
    protected virtual OTAtlasData[] Import()
    {
        return new OTAtlasData[] { };
    }

    /// <exclude />
    new protected void Update()
    {
        if (_atlasDataFile_!=atlasDataFile || reloadData)
        {
            _atlasDataFile_ = atlasDataFile;
            if (atlasDataFile != null)
            {
                atlasReady = false;
                atlasData = Import();
                atlasReady = true;
            }
            if (reloadData)
                reloadData = false;
        }

        base.Update();
    }
}