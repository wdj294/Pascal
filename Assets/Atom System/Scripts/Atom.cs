using UnityEngine;
using System.Collections.Generic;

public class Atom : MonoBehaviour {

    //Working Combos
    //5 p 0.2 scale
    //10 p 0.4 scale
    //20 p 0.6 scale

    public string elementName;
    public string elementSymbol;

    public int atomicNumber;
    public float atomicWeight;

    public int protonCount;

    public int neutronCount;
    //public Color32 neutronColor = new Color32(255, 28, 28, 255);

    public int[] electronConfiguration;
    public float electronShellSpacing = 3f;

    public bool spinElectrons = true;
    public bool freezeNucleusMovementAfter5Seconds = true;

    public void Start()
    {
        //Nucleons
        int pc = protonCount;
        int nc = neutronCount;

        float scaleCount = 0.2f;

        generateProton(new Vector3(0,0,0));
        pc -= 1;

        for (int i = 1; i <= nc + pc; i++)
        {
            int nucleonsToCreate = int.Parse((Mathf.Pow(2f, i - 1f) * 5f).ToString());

            if (nc + pc >= nucleonsToCreate)
            {
                string s = createNucleons(nucleonsToCreate, scaleCount, pc, nc);
                scaleCount += 0.2f;
                pc -= int.Parse(s.Split(',')[0]);
                nc -= int.Parse(s.Split(',')[1]);
            }
            else
            {
                string s = createNucleons(pc+nc, scaleCount, pc, nc);
                scaleCount += 0.2f;

                pc -= int.Parse(s.Split(',')[0]);
                nc -= int.Parse(s.Split(',')[1]);
            }

        }

        //Electrons
        generateElectronShells();

        for (int loop = 0; loop < electronConfiguration.Length; loop++)
        {
            int numPoints = electronConfiguration[loop];

            for (int pointNum = 0; pointNum < numPoints; pointNum++)
            {
                float i = (float)(pointNum * 1.0) / numPoints;
                float angle = i * Mathf.PI * 2;
                float x = transform.position.x + (Mathf.Sin(angle) * (3*(loop+1)));
                float z = transform.position.z + (Mathf.Cos(angle) * (3*(loop+1)));
                Vector3 pos = new Vector3(x, transform.position.y, z);
                
                generateElectron(pos, electronShellSpacing * (loop + 1), loop+1);
                atomicNumber += 1;
            }
        }

        transform.Find("Nucleus").transform.localPosition = new Vector3(transform.position.x, transform.position.y,transform.position.z);
    }

    string createNucleons(int nPoints, float scale, int pcount, int ncount)
    {
        int npc = 0;
        int nnc = 0;

        float fPoints = (float)nPoints;

        Vector3[] points = new Vector3[nPoints];

        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2 / fPoints;

        for (int k = 0; k < nPoints; k++)
        {
            float y = k * off - 1 + (off / 2);
            float r = Mathf.Sqrt(1 - y * y);
            float phi = k * inc;

            points[k] = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r);
        }

        //return points;
 
        foreach (Vector3 point in points)
        {
            if (pcount > 0 && ncount > 0)
            {
                int borc = Random.Range(1, 10);
                if (borc > 5 && pcount > 0)
                {
                    GameObject outerSphere = generateProton(transform.position);
                    outerSphere.transform.position = point * scale;
                    npc += 1;
                    pcount -= 1;
                }
                else if (borc < 6 && ncount > 0)
                {
                    GameObject outerSphere = generateNeutron(transform.position);
                    outerSphere.transform.position = point * scale;
                    nnc += 1;
                    ncount -= 1;
                }
            }

            else if (pcount > 0 && ncount == 0)
            {
                GameObject outerSphere = generateProton(transform.position);
                outerSphere.transform.position = point * scale;
                npc += 1;
                pcount -= 1;
            }

            else if (pcount == 0 && ncount > 0)
            {
                GameObject outerSphere = generateNeutron(transform.position);
                outerSphere.transform.position = point * scale;
                nnc += 1;
                ncount -= 1;
            }
        }

        return npc + "," + nnc;

    }

    bool prevSpinElectrons = true;

    public void Update()
    {
        if (spinElectrons && !prevSpinElectrons)
        {
            foreach (GameObject e in FindObjectsOfType(typeof(GameObject)))
            {
                if (e.name == "Electron")
                {
                    e.GetComponent<Electron>().enabled = true;
                    prevSpinElectrons = spinElectrons;
                }
            }
        }

        else if (!spinElectrons && prevSpinElectrons)
        {
            foreach (GameObject e in FindObjectsOfType(typeof(GameObject)))
            {
                if (e.name == "Electron")
                {
                    e.GetComponent<Electron>().enabled = false;
                    prevSpinElectrons = spinElectrons;
                }
            }
        }
    }

    public void drawCircle(float radius, LineRenderer lr)
    {
        lr.SetVertexCount(361);
        lr.useWorldSpace = false;
        lr.SetWidth(0.0005f, 0.0005f);
        lr.materials[0] = new Material(Shader.Find("Diffuse"));
        lr.materials[0].SetColor("_Color", new Color32(255, 255, 255, 255));

        float x;
        float y = 0;
        float z;

        float angle = 0;

        for (int i = 0; i < 361; i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lr.SetPosition(i, new Vector3(x, y, z));

            angle += 1;
        }
    }

    public GameObject generateProton(Vector3 pos)
    {
		GameObject proton = (GameObject)Instantiate(Resources.Load("Proton"));
        proton.name = "Proton";
		proton.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        proton.transform.position = pos;
        proton.AddComponent<Rigidbody>();
        proton.GetComponent<Rigidbody>().useGravity = false;
		proton.GetComponent<Rigidbody>().mass = 10f;
        proton.AddComponent<Nucleon>();

        proton.transform.SetParent(transform.Find("Nucleus"));

        return proton;
    }

    public GameObject generateNeutron(Vector3 pos)
    {
		GameObject neutron = (GameObject)Instantiate(Resources.Load("Neutron"));
        neutron.name = "Neutron";
		neutron.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        neutron.transform.position = pos;
        neutron.AddComponent<Rigidbody>();
        neutron.GetComponent<Rigidbody>().useGravity = false;
		neutron.GetComponent<Rigidbody>().mass = 10f;
        //neutron.GetComponent<Renderer>().material.SetColor("_Color", new Color32(255, 28, 28, 255));
        neutron.AddComponent<Nucleon>();

        neutron.transform.SetParent(transform.Find("Nucleus"));

        return neutron;
    }

    public GameObject[] generateElectronShells()
    {
        List<GameObject> shells = new List<GameObject>();

        for (int loop = 0; loop < electronConfiguration.Length; loop++)
        {
            GameObject shell = new GameObject();
            shell.name = "Shell " + (loop+1);
            shell.transform.parent = transform;
            shell.transform.localPosition = transform.Find("Nucleus").localPosition;
            LineRenderer lr = shell.AddComponent<LineRenderer>();
            drawCircle(electronShellSpacing * (loop + 1), lr);
            shells.Add(shell);
        }

        return shells.ToArray();
    }

    public void generateElectron(Vector3 pos, float r, int shell)
    {
		GameObject electron = (GameObject)Instantiate(Resources.Load("Electron"));
        electron.name = "Electron";
        electron.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        electron.transform.position = pos;
        electron.transform.parent = GameObject.Find("Shell " + shell).transform;
        GameObject.Destroy(electron.GetComponent<SphereCollider>());
        Electron e = electron.AddComponent<Electron>();
        e.radius = r;
        e.centre = transform;
        e.rotationSpeed = 8 + (10 * shell);
    }
}
