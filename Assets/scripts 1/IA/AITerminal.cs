using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class AITerminal : MonoBehaviour
{
    public Inventory playerInventory;

    [Header("Zones à revitaliser")]
    public GameObject[] zonesARevitaliser;

    [Header("UI")]
    public TextMeshProUGUI messageUI;

    [Header("Ressources requises")]
    public int besoinEau = 1;
    public int besoinGraines = 1;
    public int besoinFertilisant = 1;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip iaInteractionSound;           // 🔊 Son succès
    public AudioClip ressourcesInsuffisantesSound;  // 🔊 Son échec

    private bool joueurDansZone = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("[AITerminal] Aucun AudioSource trouvé sur ce GameObject.");
        }
        else
        {
            audioSource.playOnAwake = false; // 🛑 Pour éviter qu'un son joue au lancement
        }
    }

    void Update()
    {
        if (joueurDansZone && Input.GetKeyDown(KeyCode.E))
        {
            ActiverIA();
        }
    }

    void ActiverIA()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("[AITerminal] Aucun inventaire assigné.");
            return;
        }

        int eau = playerInventory.GetWaterDropCount();
        int graines = playerInventory.GetSeedCount();
        int fertil = playerInventory.GetFertilizerCount();

        Debug.Log($"[DEBUG] Inventaire : Eau={eau}, Graines={graines}, Fertilisant={fertil}");

        if (eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant)
        {
            foreach (GameObject zone in zonesARevitaliser)
            {
                if (zone != null)
                    zone.SetActive(true);
            }

            messageUI.text = "[ I.A LOG ] Ressources suffisantes.\nRevitalisation en cours ... trouvez la porte de sortie";

            if (audioSource != null && iaInteractionSound != null)
            {
                audioSource.PlayOneShot(iaInteractionSound);
                Debug.Log("[AITerminal] Son succès joué.");
            }
        }
        else
        {
            messageUI.text = "[ I.A LOG ] Ressources insuffisantes.\nAnalyse en attente...";

            if (audioSource != null && ressourcesInsuffisantesSound != null)
            {
                audioSource.PlayOneShot(ressourcesInsuffisantesSound);
                Debug.Log("[AITerminal] Son échec joué.");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = true;
            messageUI.text = "[ I.A LOG ] Appuyer sur E pour interagir.";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = false;
            messageUI.text = "";
        }
    }
}
