using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



public delegate void DTActionCall();
public delegate object DTDecisionCall();

public abstract class DTNode {

}




public class DTAction : DTNode {

    DTActionCall action;

    public DTAction(DTActionCall action) {
        this.action = action;
    }

    public void ExecuteAction() {
        action();
    }
}

public class DTDecision : DTNode {
    private DTDecisionCall Selector;
    private Dictionary<object, DTNode> Arcs;

    public DTDecision(DTDecisionCall selector) {
        Selector = selector;
        Arcs = new Dictionary<object, DTNode>();
    }
    public void AddNode(object arc, DTNode node) {
        Arcs.Add(arc, node);
    }
    public void takeDecision(ref DTNode node) {
        object decision = Selector();
        node = Arcs.ContainsKey(decision) ? Arcs[decision] : null;
    }
}

public class DecisionTree {
    DTNode currentNode;
    DTNode startNode;

    public DecisionTree(DTNode startNode) {
        this.startNode = startNode;
    }

    public void Walk() {
        currentNode = startNode;

        while (currentNode != null) {
            if (currentNode is DTAction) {
                //new DTAction(currentNode).ExecuteAction();
                DTAction actionNode = currentNode as DTAction;
                actionNode.ExecuteAction();
                Debug.Log("Azione eseguita");
                currentNode = null;
                return;
            }
            else if (currentNode is DTDecision) {
                DTDecision decisionNode = currentNode as DTDecision;
                decisionNode.takeDecision(ref currentNode);
            }
        }
        Debug.Log("Nessuna azione trovata");
    }

}


