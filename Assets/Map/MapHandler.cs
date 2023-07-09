using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler : MonoBehaviour
{

  // Current map
  private GameObject _currentMap;

  // Maps
  public GameObject KarugarnerPrefab;

  // Start is called before the first frame update
  void Start()
  {
    _currentMap = Instantiate(KarugarnerPrefab, this.transform);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
