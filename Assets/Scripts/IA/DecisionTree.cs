using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



public delegate void DTCall();
public delegate object DTDecisionCall();

public interface DTNode {
    void walk(ref DTNode currentNode);
}


public class DTAction : DTNode {

    DTCall Action;

    public DTAction(DTCall action) {
        Action = action;
    }

    public void walk(ref DTNode currentNode) {
        Action();
        currentNode = null;
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
    public void walk(ref DTNode currentNode) {
        object decision = Selector();
        currentNode = Arcs.ContainsKey(decision) ? Arcs[decision] : null;
        if (currentNode == null) Debug.Log("Nessuna azione trovata");
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
            currentNode.walk(ref currentNode);           
        }
    }

}


