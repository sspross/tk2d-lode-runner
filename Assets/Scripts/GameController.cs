using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class GameController : MonoBehaviour {
	
	public int gridWidth = 40;
	public int gridHeight = 30;
	public int gridSize = 30;
	
	public Transform solid, brick, ladder, player, pickup, rope;
	
	public int pickups = 0;

	void Start () {
    	try {
            using (StreamReader sr = new StreamReader(Application.dataPath + "/Levels/" + "egli01.txt")) {
                string line;
				int gridY = gridHeight-1;
				
                while ((line = sr.ReadLine()) != null) {
					// if line.Lenght != gridWidth-1 and other conditions
                    char[] chars = new char[gridWidth];
					using (StringReader sgr = new StringReader(line)) {
			            sgr.Read(chars, 0, gridWidth);
						int gridX = 0;
			            foreach (char c in chars) {
							CreateInstance(c, gridX, gridY);
							gridX++;
						}
        			}
					gridY--;
                }
            }
        } catch (Exception e) {
            Debug.LogException(e);
        }
	}
	
	void CreateInstance(char c, int x, int y) {
		x *= gridSize;
		y *= gridSize;
		switch (char.ToLower(c)) {
		    case 'x':
				Instantiate(solid, new Vector3(x, y, 0), Quaternion.identity);
				break;
			case '#':
				Instantiate(brick, new Vector3(x, y, 0), Quaternion.identity);
				break;
			case 'h':
				Instantiate(ladder, new Vector3(x, y, 0), Quaternion.identity);
				break;
			case '-':
				Instantiate(rope, new Vector3(x, y, 0), Quaternion.identity);
				break;
			case 'o':
				Instantiate(pickup, new Vector3(x, y, 0), Quaternion.identity);
				pickups++;
				break;
			case 'p':
				Instantiate(player, new Vector3(x, y, -1), Quaternion.identity);
				break;
		}	
	}
	
	void PickedUp() {
		pickups--;	
	}
	
}
