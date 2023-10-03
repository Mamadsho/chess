using UnityEngine;


// Pay attention to Script Execution Order

[RequireComponent (typeof(BoardMove))]
public class Highlights : MonoBehaviour
{
    [SerializeField] GameObject validReticle;
    [SerializeField] GameObject selectedReticle;
    [SerializeField] Material validMoveMaterial;
    [SerializeField] Material killMoveMaterial;
    [SerializeField] Material specialMoveMaterial;
    [SerializeField] UnityEngine.Rendering.Volume volume;
    [SerializeField] GameObject checkMate;
    private GameObject [,] reticles = new GameObject [8, 8];


    GameManager gameManager;
    private GameObject reticlesHolder;
    private int[,] validMoves;
    private Material[] reticleMaterials = new Material[6];

    void Start()
    {
        reticleMaterials[0] = null;
        reticleMaterials[1] = validMoveMaterial;
        reticleMaterials[2] = killMoveMaterial;
        reticleMaterials[3] = specialMoveMaterial;
        reticleMaterials[4] = specialMoveMaterial;
        reticleMaterials[5] = specialMoveMaterial;

        gameManager = GameManager.Singleton;
        reticlesHolder = new GameObject("ReticlesHolder");
        reticlesHolder.transform.SetParent(gameManager.boardCoordinateSystem, false);
        reticlesHolder.SetActive(false);

        selectedReticle = Instantiate(selectedReticle, reticlesHolder.transform, false);

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject reticle = Instantiate(validReticle, reticlesHolder.transform, false);
                reticles[x, y] = reticle;
                reticle.transform.localPosition = new Vector3(x , y, 0);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            // Vignette
            if (gameManager.KingInCheck('w')||gameManager.KingInCheck('b')) {
                ChangeVignette(Color.red);
            } else {
                ChangeVignette(Color.black);
            }
            if (gameManager.CheckMate()) 
            { 
                ChangeVignette(Color.red);
                checkMate.SetActive(true);
            }

            if (gameManager.selectedPiece)
            {
                reticlesHolder.SetActive(true);
                selectedReticle.transform.localPosition = gameManager.selectedPiece.transform.localPosition - new Vector3(.5f, .5f, 0);
                validMoves = gameManager.GetValidMoves(gameManager.selectedPiece);
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        reticles[x, y].SetActive(validMoves[x, y] != 0);
                        reticles[x, y].GetComponent<MeshRenderer>().material = reticleMaterials[validMoves[x, y]]; 
                    }
                }
            }
            else
            {
                reticlesHolder.SetActive(false);
            }
        }
    }

    void ChangeVignette(Color color)
    {
        UnityEngine.Rendering.VolumeProfile volumeProfile = volume.GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));

        // You can leave this variable out of your function, so you can reuse it throughout your class.
        UnityEngine.Rendering.Universal.Vignette vignette;

        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));

        // vignette.intensity.Override(0.5f);
        // vignette.color.Override(Color.red);
        vignette.color.Interp(vignette.color.value, color, 1);
    }
}


