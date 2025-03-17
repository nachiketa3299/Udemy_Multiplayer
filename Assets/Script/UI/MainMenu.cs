using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
  [SerializeField] TMP_InputField _joinCodeField;
  public async void StartHost()
  {
    await HostSingleton.Instance.GameManager.StartHostAsync();
  }

  public async void StartClient()
  {
    await ClientSingleton.Instance.GameManager.StartClientAsync(_joinCodeField.text);
  }
}
