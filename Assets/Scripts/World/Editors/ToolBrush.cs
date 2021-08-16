using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Challenge = Room.Challenge;

public class ToolBrush : MonoBehaviour {

    /* --- COMPONENTS --- */
    public Select emptyBrush;
    public ToolSet toolSet;
    public Layout interiorLayout;

    // the current room tags
    public SpriteRenderer currShapeTag;
    public Sprite[] shapeTags;

    public SpriteRenderer currPathTag;
    public Sprite[] pathTags;

    public SpriteRenderer currChallengeTag;
    public Sprite[] challengeTags;

    /* --- VARIABLES --- */
    Select[] brushes = new Select[0];

    public Select[] SetPalette(Challenge challenge) {

        // clean up the previous set of brushes
        for (int i = brushes.Length - 1; i >= 0; i--) {
            Destroy(brushes[i].gameObject);
        }

        // im doing this a bit weirdly but its pre cool
        int maxID = 0;
        switch (challenge) {
            case Challenge.COMBAT:
                maxID = SetBrushesFromArray(toolSet.mobs);
                break;
            case Challenge.TRAP:
                maxID = SetBrushesFromArray(toolSet.traps);
                break;
            default:
                brushes = new Select[0];
                break;
        }

        interiorLayout.SetLayoutFromBrushes(brushes, maxID);
        return brushes;

    }

    int SetBrushesFromArray(Controller[] controllers) {

        brushes = new Select[controllers.Length];
        for (int i = 0; i < controllers.Length; i++) {

            // the position of the new brush
            Vector3 position = emptyBrush.transform.position + new Vector3(i % 3, Mathf.Floor(i / 3), 0);

            // instanstiate the new brush
            Select newBrush = Instantiate(emptyBrush.gameObject, position, Quaternion.identity, transform).GetComponent<Select>();

            // set the appropriate display for the brush
            newBrush.GetComponent<SpriteRenderer>().sprite = controllers[i].state._renderer.defaultSprite;

            // set the correct value for the brush
            newBrush.index = controllers[i].id;
            brushes[i] = newBrush;
        }

        return toolSet.GetMaxControllerID(controllers);
    }

    

}
