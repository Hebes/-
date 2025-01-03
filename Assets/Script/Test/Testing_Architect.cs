using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Testing_Architect : BaseBehaviour
{
    private TextArchitect _architect;
    public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

    private string[] line = new[]
    {
        "hello1",
        "hello2hello2",
        "hello3hello3hello3",
    };

    private void Start()
    {
        _architect = new TextArchitect();
        _architect.buildMethod = TextArchitect.BuildMethod.fade;
        _architect.speed = 0.5f;
    }

    private void Update()
    {
        if (bm != _architect.buildMethod)
        {
            _architect.buildMethod = bm;
            _architect.Stop();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _architect.Stop();
        }

        string str = "111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111";
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_architect.isBuilding)
            {
                if (!_architect.hurryUp)
                    _architect.hurryUp = true;
                else
                    _architect.ForceComplete();
            }
            else
            {
                //_architect.Build(line[Random.Range(0, line.Length)]);
                _architect.Build(str);
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            //_architect.Append(line[Random.Range(0, line.Length)]);
            _architect.Append(str);
        }
    }
}