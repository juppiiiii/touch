using UnityEngine;

public class SCControl : MonoBehaviour {
	public ObjectManager objectManager;
	public GameObject manager;
	private Material material;
	private float originOutLineWidth;
	private float highlightOutLineWidth = 0.03f;
	public GameObject obj;

<<<<<<< Updated upstream
	//  ½×±â °¡´ÉÇÑ ¿ÀºêÁ§Æ®¿¡°Ô ºÎ¿©µÇ´Â ¼Ó¼º
	public bool ableStack = true;

	/// ¾²·¹±âÅë°ú ¼¼Å¹±âÀÇ ¿µ¿ª
=======
	/// ì“°ë ˆê¸°í†µê³¼ ì„¸íƒê¸°ì˜ ì˜ì—­
>>>>>>> Stashed changes
	private float washMinX = 0;
	private float washMaxX = 10;
	private float washMinZ = -20;
	private float washMaxZ = -10;
	private float trashMinX = 10;
	private float trashMaxX = 20;
	private float trashMinZ = -10;
	private float trashMaxZ = 0;

	void Start()
	{
		// ¿ÀºêÁ§Æ®ÀÇ Material º¹Á¦ÇÏ±â
		material = new Material(GetComponent<Renderer>().material);
		GetComponent<Renderer>().material = material;
		originOutLineWidth = material.GetFloat("_OutlineWidth");
		manager = GameObject.Find("ObjectManager");
		objectManager = manager.GetComponent<ObjectManager>();
	}
	private void Update()
	{
		DestroyNC();
		Vector3 pos = obj.transform.position;
		if (!objectManager.leftHold)
		{
			//Á¤¸®°¡ ÀßµÈ °æ¿ì: ¿ÀºêÁ§Æ® Á¦°Å(NC±âÁØ)
			if (pos.x <= washMaxX && pos.x >= washMinX && pos.z <= washMaxZ && pos.z >= washMinZ)
			{
				//¿ÀºêÁ§Æ® Á¦°Å ¹× obj null·Î º¯°æ
				Destroy(gameObject);
				obj = null;
				return;
			}
			//Á¤¸®°¡ Àß¸øµÈ °æ¿ì - ¿ÀºêÁ§Æ® Á¦°Å(»ç½Ç ÇÏ´Â ÀÏ ¶È°°Àºµ¥ º¸±â ¾îÁö·¯¿ö¼­, È¤½Ã ´Ù¸¥ ±â´ÉÀÌ »ý±æ ¼ö ÀÖÀ¸¹Ç·Î µû·Î ºÐ¸®)
			if (pos.x <= trashMaxX && pos.x >= trashMinX && pos.z <= trashMaxZ && pos.z >= trashMinZ)
			{
				Destroy(gameObject);
				obj = null;
				return;
			}
		}
	}

	void OnMouseOver()
	{
		material.SetFloat("_OutlineWidth", highlightOutLineWidth);
		//Debug.Log("¸¶¿ì½º °¨Áö. ÇöÀç Å×µÎ¸® µÎ²²: " + material.GetFloat("_OutlineWidth"));
	}

	void OnMouseExit()
	{
		material.SetFloat("_OutlineWidth", originOutLineWidth);
		//Debug.Log("¸¶¿ì½º °¨Áö Á¾·á. ÇöÀç Å×µÎ¸® µÎ²²: " + material.GetFloat("_OutlineWidth"));
	}

	void DestroyNC()
	{
		if (Input.GetKeyDown("x"))
		{
			Destroy(gameObject);
			obj = null;
		}
	}
}
