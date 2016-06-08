using UnityEngine;
using System.Collections;

public class CrowdIA : MonoBehaviour {

    DecisionTree CrowdTree;
    // Use this for initialization
    void Start() {
        DTDecision d1 = new DTDecision(randomNumber);


        CrowdTree = new DecisionTree(d1);
        DTAction a1 = new DTAction(trueOption);
        DTAction a2 = new DTAction(falseOption);
        d1.AddNode(true, a1);
        d1.AddNode(false, a2);
        StartCoroutine(Patrol(CrowdTree));

    }

    // Update is called once per frame
    void Update() {

    }

    object randomNumber() {
        return Random.value > 0.5 ? true : false;
    }

    void trueOption() {
        Debug.Log("Opzione true");
    }

    void falseOption() {
        Debug.Log("Opzione false");
    }

    IEnumerator Patrol(DecisionTree tree) {
        while (true) {
            tree.Walk();
            yield return new WaitForSeconds(2f);
        }
    }


}
