using System.Runtime.Serialization;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	//
	private bool isCorrect = true;
	public GameObject selected;
	private bool isDragging = false;
	private Vector3 lastMousePosition;
	private Vector3 latestPos;
	public string selectedTag;
	private Camera mainCamera;
	public bool leftHold = false;

<<<<<<< Updated upstream
	//°ÔÀÓ¸Å´ÏÀú
=======
	//ìŒ“ê¸° ê°€ëŠ¥í•œ ë¬¼ê±´ì„ ë¯¸ë¦¬ ì •í•´ë‘ 
	private List<string> stackAble = new List<string>(){ "CleanBed", "Bookcase", "LowBookcase", "WrappedBox", "WrappedSB", "BottleWater", "OpenedBook", "FO", "NT"};

	//ê²Œì„ë§¤ë‹ˆì €
>>>>>>> Stashed changes
	public GameObject gm;
	public GameManager gameManager;

	// ÀÌµ¿ °¡´É ¿µ¿ª ÇÑ°è
	private float maxZ = -0.5f;
	private float minZ = -19.5f;
	private float minX = 0.5f;
	private float maxX = 19.5f;

	// ¾²·¹±âÅë°ú ¼¼Å¹±âÀÇ ¿µ¿ª
	private float washMinX = 0;
	private float washMaxX = 10;
	private float washMinZ = -20;
	private float washMaxZ = -10;
	private float trashMinX = 10;
	private float trashMaxX = 20;
	private float trashMinZ = -10;
	private float trashMaxZ = 0;

	// WellDestroyed°¡ 1È¸¸¸ È£ÃâµÇµµ·Ï Á¦¾îÇÏ´Â ÇÃ·¡±× º¯¼ö
	private bool wellDestroyedCalled = false;

	// ¿ìÅ¬¸¯ ±æ°Ô ´©¸£±â À§ÇÑ º¯¼öµé
	public bool ableInterection = false;           // 3ÃÊ µ¿¾È ¿ìÅ¬¸¯ À¯Áö ½Ã true°¡ µÊ
	public string interactionWith = "";             // »óÈ£ÀÛ¿ë ½ÇÇà Ç×¸ñÀ» ¾Ë¸®±â À§ÇÑ ¹®ÀÚ¿­ º¯¼ö
	private float rightClickTimer = 0f;             // ¿ìÅ¬¸¯ Áö¼Ó½Ã°£ Ã¼Å©¿ë Å¸ÀÌ¸Ó
	public float rightClickHoldTime = 3f;           // 3ÃÊ°¡ µÇ¾î¾ß È¿°ú ¹ß»ı
	private bool isRightClickHeld = false;          // ÇöÀç ¿ìÅ¬¸¯ÀÌ À¯ÁöµÇ°í ÀÖ´ÂÁö ¿©ºÎ

	private void Start()
	{
		mainCamera = Camera.main;
		gm = GameObject.Find("GameObject");
		gameManager = gm.GetComponent<GameManager>();
	}

	private void Update()
	{
		//³·¿¡¸¸ ¸¶¿ì½º ÀÌµ¿ÀÌ °¡´ÉÇÏµµ·Ï ÇÑÁ¤.
		//if (!gameManager.IsNight) >> ³·ÀÌ ¹İ¿µµÉ ¶§ºÎÅÍ Àû¿ëÇÏ±æ...
		{
			LeftControl();
			RightControl();

			//¼±ÅÃµÈ ¿ÀºêÁ§Æ®°¡ ÀÖÀ» °æ¿ì ÀÌµ¿ Ã³¸®
			if (selected != null)
			{
				if (isDragging)
				{
					DragMove();
				}
				/*else >> ¹ã¿¡ ¿òÁ÷ÀÌ´Â Àå³­°¨µéÀ» À§ÇÑ ÀÌµ¿. ¿Å±æ ¿¹Á¤
				{
					WASDMove();
				}*/
				Clamping();
			}

			// ¿ÀºêÁ§Æ® ÆÄ±« °¨Áö: ÆÄ±«µÇ°í ¾ÆÁ÷ WellDestroyed()¸¦ È£ÃâÇÏÁö ¾Ê¾Ò´Ù¸é 1È¸¸¸ ½ÇÇà
			if (selected == null && !wellDestroyedCalled)
			{
				wellDestroyedCalled = true;
				isCorrect = WellDestroyed();
				Debug.Log("WellDestroyed °á°ú: " + isCorrect);
			}
		}
	}

	void LeftControl()
	{
		// ¸¶¿ì½º ÁÂÅ¬¸¯ Ã³¸®
		if (Input.GetMouseButtonDown(0))
		{
			FindLeftClick();
		}
		if (Input.GetMouseButtonUp(0))
		{
			LostLeftClick();
		}
	}

	// ÁÂÅ¬¸¯ °¨Áö: ¿ÀºêÁ§Æ® ¼±ÅÃ ¹× µå·¡±× ½ÃÀÛ
	void FindLeftClick()
	{
		RaycastHit hit;

		int ignoreLayers = LayerMask.GetMask("Ignore Raycast", "Hover");
		int layerMask = ~ignoreLayers;  // À§ µÎ ·¹ÀÌ¾î¸¦ Á¦¿ÜÇÑ ¸ğµç ·¹ÀÌ¾î

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

		if (hits.Length > 0 && Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if (hit.transform != null)
			{
				selected = hit.transform.gameObject; // Å¬¸¯ÇÑ ¿ÀºêÁ§Æ® ÀúÀå
				//Debug.Log("Selected Object: " + selected.name); // µğ¹ö±ë¿ë

				// µå·¡±× ½ÃÀÛ ¼³Á¤-> tag·Î ¿Å±æ ¼ö ÀÖ´Â ¹°°ÇÀÎÁö ÆÇº°
				if (selected.tag != "FO")
				{
					isDragging = true;
					selectedTag = selected.tag;
					leftHold = true;

					// ¸¶¿ì½º ÀÌÀü À§Ä¡ ÃÊ±âÈ­
					lastMousePosition = Input.mousePosition;

					// »õ ¿ÀºêÁ§Æ® ¼±ÅÃ ½Ã WellDestroyed È£Ãâ ÇÃ·¡±× ÃÊ±âÈ­
					wellDestroyedCalled = false;
				}
				
			}
		}
	}

	// ÁÂÅ¬¸¯ ÇØÁ¦ °¨Áö
	void LostLeftClick()
	{
		if (isDragging)
		{
			isDragging = false;
			leftHold = false;
			Debug.Log("µå·¡±× Á¾·á");
		}
	}

	// µå·¡±× ÀÌµ¿ Ã³¸®
	void DragMove()
	{
		Plane dragPlane = new Plane(Vector3.up, selected.transform.position);
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

		if (dragPlane.Raycast(ray, out float enter))
		{
			Vector3 hitPoint = ray.GetPoint(enter);
			Vector3 worldDelta = hitPoint - selected.transform.position;
			worldDelta.y = 0f;
			selected.transform.position += worldDelta;
		}
		lastMousePosition = selected.transform.position;
	}

	// WASD Å° ÀÌµ¿ Ã³¸®(Àå³­°¨ ÇÑÁ¤.)
	void WASDMove()
	{
		if (Input.GetKeyDown("w"))
		{
			Vector3 locate = selected.transform.position;
			selected.transform.position = new Vector3(locate.x, locate.y, locate.z + 1);
		}
		if (Input.GetKeyDown("a"))
		{
			Vector3 locate = selected.transform.position;
			selected.transform.position = new Vector3(locate.x - 1, locate.y, locate.z);
		}
		if (Input.GetKeyDown("s"))
		{
			Vector3 locate = selected.transform.position;
			selected.transform.position = new Vector3(locate.x, locate.y, locate.z - 1);
		}
		if (Input.GetKeyDown("d"))
		{
			Vector3 locate = selected.transform.position;
			selected.transform.position = new Vector3(locate.x + 1, locate.y, locate.z);
		}
	}

	//¸¶¿ì½º ¿ìÅ¬¸¯ Á¦¾î
	void RightControl()
	{
		if (Input.GetMouseButton(1))
			FindRightClick();

		// ¸¶¿ì½º ¿ìÅ¬¸¯ ±æ°Ô ´©¸£±â Ã³¸®, selected°¡ ÀÖ¾î¾ß ÀÇ¹Ì°¡ ÀÖÀ½.
		if (Input.GetMouseButtonDown(1) && selected != null)
		{
			isRightClickHeld = true;
			rightClickTimer = 0f;
			// ¿©±â¼­ ¿øÇü °ÔÀÌÁö °´Ã¼¸¦ »ı¼ºÇÒ ¼ö ÀÖÀ½.
			gameManager.StartFillingInteractionGauge(3, 3.3f);
			
		}

		if (isRightClickHeld)
		{
			rightClickTimer += Time.deltaTime;

			// 3ÃÊ ÀÌ»ó ´­·¶°í ¾ÆÁ÷ »óÈ£ÀÛ¿ë ½ÇÇàÀÌ ¾ÈµÈ °æ¿ì
			if (rightClickTimer >= rightClickHoldTime && !ableInterection)
			{
				interactionWith = $"inter_{selected.name}";
				Debug.Log($"inter_{selected.name}");
				ableInterection = true;
				// 3ÃÊ µµ´Ş ÈÄ °ÔÀÌÁö´Â ´õ ÀÌ»ó ÁøÇàµÇÁö ¾ÊÀ½
				isRightClickHeld = false;
			}
		}

		if (Input.GetMouseButtonUp(1))
		{
			// ¿ìÅ¬¸¯ ÇØÁ¦ ½Ã Å¸ÀÌ¸Ó¿Í »óÅÂ ÃÊ±âÈ­
			isRightClickHeld = false;
			rightClickTimer = 0f;
			interactionWith = "";
			ableInterection = false;
			Debug.Log($"gauge : {gameManager.InteractionGauge}");
		}
	}

	void FindRightClick()
	{
		RaycastHit hit;

		int ignoreLayers = LayerMask.GetMask("Ignore Raycast", "Hover");
		int layerMask = ~ignoreLayers;  // À§ µÎ ·¹ÀÌ¾î¸¦ Á¦¿ÜÇÑ ¸ğµç ·¹ÀÌ¾î

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

		if (hits.Length > 0 && Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
		{
			if (hit.transform != null)
			{
				selected = hit.transform.gameObject; // Å¬¸¯ÇÑ ¿ÀºêÁ§Æ® ÀúÀå
				//Debug.Log("Selected Object: " + selected.name); // µğ¹ö±ë¿ë

				// ¸¶¿ì½º ÀÌÀü À§Ä¡ ÃÊ±âÈ­
				lastMousePosition = Input.mousePosition;
			}
			else
			{
				selected = null;
			}
		}
	}

	// ¿ÀºêÁ§Æ® À§Ä¡ Á¦ÇÑ
	void Clamping()
	{
		Vector3 pos = selected.transform.position;
		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
		selected.transform.position = latestPos = pos;
	}

<<<<<<< Updated upstream
	// ¿Ã¹Ù¸¥ À§Ä¡¿¡ µµ´ŞÇß´ÂÁö °Ë»çÇÏ´Â ÇÔ¼ö
=======

	//ì˜¤ë¸Œì íŠ¸ì˜ ê²¹ì¹¨ ë°©ì§€
	void ResolveOverlaps()
	{
		// selected ì˜¤ë¸Œì íŠ¸ê°€ ë°˜ë“œì‹œ Colliderë¥¼ ê°€ì§€ê³  ìˆì–´ì•¼ í•¨
		Collider selectedCollider = selected.GetComponent<Collider>();
		if (selectedCollider == null)
		{
			return;
		}
			

		// selectedì˜ í˜„ì¬ ìœ„ì¹˜ì—ì„œ ê²¹ì¹˜ëŠ” ë‹¤ë¥¸ Colliderë“¤ì„ ì°¾ìŒ
		Collider[] overlappingColliders = Physics.OverlapBox(
			selectedCollider.bounds.center,
			selectedCollider.bounds.extents,
			selected.transform.rotation);

		// ê° ê²¹ì¹˜ëŠ” ì˜¤ë¸Œì íŠ¸ì— ëŒ€í•´ ë¶„ë¦¬ ë²¡í„°ë¥¼ ê³„ì‚°
		foreach (Collider other in overlappingColliders)
		{
			if (other.gameObject == selected)
				continue; // ìê¸° ìì‹ ì€ ì œì™¸

			Vector3 direction;
			//Debug.Log($"other : {other.transform.position.x} {other.transform.position.y} {other.transform.position.z}");
			float distance;
			// ë‘ Colliderê°€ ê²¹ì¹˜ëŠ”ì§€ ê²€ì‚¬í•˜ê³ , ê²¹ì¹¨ ì •ë„ë¥¼ ê³„ì‚°
			bool isOverlapping = Physics.ComputePenetration(
				selectedCollider,
				selected.transform.position,
				selected.transform.rotation,
				other,
				other.transform.position,
				other.transform.rotation,
				out direction,
				out distance);

			if (isOverlapping)
			{
				Debug.Log(stackAble.Contains(selected.name));
				Debug.Log(stackAble.Contains(other.name));
				// ìŒ“ê¸°ê°€ ê°€ëŠ¥í•œ ê²½ìš°(ì–´ë–»ê²Œ ì ‘ê·¼í• ì§€ ê³ ë¯¼ì¤‘)
				if (stackAble.Contains(selected.name) && stackAble.Contains(other.name))
				{
					Debug.Log(distance);
					Vector3 dist = new Vector3(selected.transform.localScale.x - other.transform.localScale.x, other.transform.localScale.z, selected.transform.localScale.z - other.transform.localScale.z);
					Vector3 gap = new Vector3(selected.transform.position.x - other.transform.position.x, 0, selected.transform.position.z - other.transform.position.z);
					Debug.Log($"other : {direction.x} {direction.y} {direction.z}");
					// direction(ë²•ì„ )ì™€ distanceë¥¼ ì´ìš©í•´ selected ì˜¤ë¸Œì íŠ¸ë¥¼ ë°€ì–´ëƒ„
					selected.transform.position += (dist - gap) * (distance + 1f);
				}
				//ìŒ“ê¸°ê°€ ë¶ˆê°€ëŠ¥í•œ ê²½ìš° - ì˜†ìœ¼ë¡œ ë°€ì–´ë‚¸ë‹¤.(ê²¹ì¹˜ì§€ ì•Šë„ë¡)
				else
				{
					Vector3 gap = new Vector3(selected.transform.position.x - other.transform.position.x, 0, selected.transform.position.z - other.transform.position.z);Debug.Log($"{gap.x}, {gap.z}");
					Vector3 dist = -gap.x < gap.z ? new Vector3(other.transform.localScale.x, 0, 0) : new Vector3(0, 0, -other.transform.localScale.y);
					
					Debug.Log($"other : {direction.x} {direction.y} {direction.z}");
					// direction(ë²•ì„ )ì™€ distanceë¥¼ ì´ìš©í•´ selected ì˜¤ë¸Œì íŠ¸ë¥¼ ë°€ì–´ëƒ„
					selected.transform.position += (dist - gap) * (distance + 1f);
				}
			}
		}
	}
	// ì˜¬ë°”ë¥¸ ìœ„ì¹˜ì— ë„ë‹¬í–ˆëŠ”ì§€ ê²€ì‚¬í•˜ëŠ” í•¨ìˆ˜
>>>>>>> Stashed changes
	bool WellDestroyed()
	{
		Debug.Log($"{lastMousePosition.x} {lastMousePosition.y} {lastMousePosition.z}");
		// ¼±ÅÃµÈ ÅÂ±×¿¡ µû¶ó °Ë»ç
		if (selectedTag == "NC" || selectedTag == "SC")
		{
			if (lastMousePosition.x <= washMaxX && lastMousePosition.x >= washMinX &&
				lastMousePosition.z <= washMaxZ && lastMousePosition.z >= washMinZ)
				return true;
			else
				return false;
		}
		else
		{
			if (lastMousePosition.x <= trashMaxX && lastMousePosition.x >= trashMinX &&
				lastMousePosition.z <= trashMaxZ && lastMousePosition.z >= trashMinZ)
				return true;
			else
				return false;
		}
	}
}
