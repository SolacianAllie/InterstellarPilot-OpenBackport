using UnityEngine;

public class TranslateTextureOffset : MonoBehaviour
{
	private Material material;

	public Vector2 ScrollRate = Vector2.zero;

	private Vector2 textureOffset = Vector2.zero;

	public bool UseRealTime;

	private void Start()
	{
		material = GetComponent<Renderer>().material;
	}

	private void Update()
	{
		float num = (UseRealTime ? RealTime.deltaTime : Time.deltaTime);
		textureOffset += ScrollRate * num;
		material.SetTextureOffset("_MainTex", textureOffset);
	}
}
