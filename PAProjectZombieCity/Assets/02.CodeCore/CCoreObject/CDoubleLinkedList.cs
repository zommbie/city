using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CDoubleLinkedList<TEMPLATE>
{
    private CDoubleLinkedListNode<TEMPLATE> m_pFirstNode;               public CDoubleLinkedListNode<TEMPLATE> First { get { return m_pFirstNode; } }
    private CDoubleLinkedListNode<TEMPLATE> m_pLastNode;                public CDoubleLinkedListNode<TEMPLATE> Last { get { return m_pLastNode; } }
    private int m_iCount = 0;                                           public int Count { get { return m_iCount; } }

     
    public Enumerator AddFirst(CDoubleLinkedListNode<TEMPLATE> node)
    {
        if (node == null) return default;
        if (node.List != null) return default;

        PrivDoubleLinkedListAdd(null, node, m_pFirstNode);
        return GetEnumerator();
    }
    public Enumerator AddFirst(TEMPLATE value)
    {
        CDoubleLinkedListNode<TEMPLATE> newNode = new CDoubleLinkedListNode<TEMPLATE>(value);

        PrivDoubleLinkedListAdd(null, newNode, m_pFirstNode);
        return GetEnumerator();
    }
    public Enumerator AddLast(CDoubleLinkedListNode<TEMPLATE> node)
    {
        if (node == null) return default;
        if (node.List != null) return default;

        PrivDoubleLinkedListAdd(m_pLastNode, node, null);
        return GetLastEnumerator();
    }
    public Enumerator AddLast(TEMPLATE value)
    {
        CDoubleLinkedListNode<TEMPLATE> newNode = new CDoubleLinkedListNode<TEMPLATE>(value);

        PrivDoubleLinkedListAdd(m_pLastNode, newNode, null);

        return GetLastEnumerator();
    }
    public void Clear()
    {
        PrivDoubleLinkedListClear();
        m_iCount = 0;
        m_pFirstNode = null;
        m_pLastNode = null;
    }
    public bool Contains(TEMPLATE value)
    {
        Enumerator enumerator = PrivDoubleLinkedListFind(value);
        bool bContain = enumerator.CanUse();
        return bContain;
    }
    public Enumerator Find(TEMPLATE value)
    {
        return PrivDoubleLinkedListFind(value);
    }
    public Enumerator FindLast(TEMPLATE value)
    {
        return PrivDoubleLinkedListFindLast(value);
    }
    public void Remove(CDoubleLinkedListNode<TEMPLATE> node)
    {
        if (node == null) return;
        if (node.List != this) return;

        PrivDoubleLinkedListRemove(node);
    }
    public bool Remove(TEMPLATE value)
    {
        Enumerator enumerator = PrivDoubleLinkedListFind(value);
        if (!enumerator.CanUse()) return false;

        enumerator.Remove();
        return true;
    }
    public void RemoveFirst()
    {
        if (m_pFirstNode == null) return;
        PrivDoubleLinkedListRemove(m_pFirstNode);
    }
    public void RemoveLast()
    {
        if (m_pLastNode == null) return;
        PrivDoubleLinkedListRemove(m_pLastNode);
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    public Enumerator GetLastEnumerator()
    {
        return new Enumerator(this, true);
    }

    public void CopyTo(TEMPLATE[] array, int arrayIndex)
    {
        if (array == null) return;
        if (array.Length <= arrayIndex || 0 > arrayIndex) return;
        if (array.Length - arrayIndex < m_iCount) return;

        CDoubleLinkedListNode<TEMPLATE> node = m_pFirstNode;

        int index = arrayIndex;
        while (node != null)
        {
            array[index] = node.Value;
            node = node.Next;
            index++;
        }

    }

    //---------------------------------------------------------------------------------------
    internal void InterAddAfter(CDoubleLinkedListNode<TEMPLATE> node, CDoubleLinkedListNode<TEMPLATE> newNode)
    {
        if (node == null || newNode == null) return;
        if (node.List != this) return;
        if (newNode.List != null) return;

        PrivDoubleLinkedListAdd(node, newNode, node.Next);
    }
    internal CDoubleLinkedListNode<TEMPLATE> InterAddAfter(CDoubleLinkedListNode<TEMPLATE> node, TEMPLATE value)
    {
        if (node == null) return null;
        if (node.List != this) return null;

        CDoubleLinkedListNode<TEMPLATE> newNode = new CDoubleLinkedListNode<TEMPLATE>(value);
        PrivDoubleLinkedListAdd(node, newNode, node.Next);

        return newNode;
    }
    internal void InterAddBefore(CDoubleLinkedListNode<TEMPLATE> node, CDoubleLinkedListNode<TEMPLATE> newNode)
    {
        if (node == null || newNode == null) return;
        if (node.List != this) return;
        if (newNode.List != null) return;

        PrivDoubleLinkedListAdd(node.Previous, newNode, node);
    }
    internal CDoubleLinkedListNode<TEMPLATE> InterAddBefore(CDoubleLinkedListNode<TEMPLATE> node, TEMPLATE value)
    {
        if (node == null) return null;
        if (node.List != this) return null;

        CDoubleLinkedListNode<TEMPLATE> newNode = new CDoubleLinkedListNode<TEMPLATE>(value);
        PrivDoubleLinkedListAdd(node.Previous, newNode, node);

        return newNode;
    }

    //--------------------------------------------------------------------------------------

    private void PrivDoubleLinkedListAdd(CDoubleLinkedListNode<TEMPLATE> previousNode, CDoubleLinkedListNode<TEMPLATE> newNode, CDoubleLinkedListNode<TEMPLATE> nextNode)
    {
        newNode.InterSetPrevious(previousNode);
        newNode.InterSetNext(nextNode);
        newNode.InterSetOwner(this);
        m_iCount++;

        if (previousNode != null)
        {
            previousNode.InterSetNext(newNode);
        }
        else
        {
            m_pFirstNode = newNode;
        }

        if (nextNode != null)
        {
            nextNode.InterSetPrevious(newNode);
        }
        else
        {
            m_pLastNode = newNode;
        }
    }

    private Enumerator PrivDoubleLinkedListFind(TEMPLATE value)
    {
        if (m_pFirstNode == null) return default(Enumerator);

        Enumerator enumerator = GetEnumerator();

        while(enumerator.MoveNext())
        {
            if (enumerator.Current.Equals(value))
            {
                return enumerator;
            }
        }
        return default(Enumerator);
    }
    private Enumerator PrivDoubleLinkedListFindLast(TEMPLATE value)
    {
        if (m_pLastNode == null) return default(Enumerator);

        Enumerator enumerator = GetLastEnumerator();

        while (enumerator.MovePrev())
        {
            if (enumerator.Current.Equals(value))
            {
                return enumerator;
            }
        }
        return default(Enumerator);
    }

    private void PrivDoubleLinkedListRemove(CDoubleLinkedListNode<TEMPLATE> node)
    {
        CDoubleLinkedListNode<TEMPLATE> previousNode = node.Previous;
        CDoubleLinkedListNode<TEMPLATE> nextNode = node.Next;
        if (previousNode == null)
        {
            m_pFirstNode = nextNode;
        }
        else
        {
            previousNode.InterSetNext(nextNode);
        }

        if (nextNode == null)
        {
            m_pLastNode = previousNode;
        }
        else
        {
            nextNode.InterSetPrevious(previousNode);
        }
        node.InterSetOwner(null);
        node.InterSetNext(null);
        node.InterSetPrevious(null);
        m_iCount--;
    }

    private void PrivDoubleLinkedListClear()
    {
        if (m_pFirstNode == null) return;

        CDoubleLinkedListNode<TEMPLATE> node = m_pFirstNode;
        while (node != null)
        {
            CDoubleLinkedListNode<TEMPLATE> nextNode = node.Next;
            node.InterSetOwner(null);
            node.InterSetPrevious(null);
            node.InterSetNext(null);
            node = nextNode;
        }
    }

    private CDoubleLinkedListNode<TEMPLATE> PrivDoubleLinkedListGetNode(int index)
    {
        if (index >= m_iCount || index < 0) return null;

        CDoubleLinkedListNode<TEMPLATE> node = m_pFirstNode;
        for (int i = 0; i < index; i++)
        {
            node = node.Next;
        }
        return node;
    }

    //--------------------------------------------------------------------------------------

    public TEMPLATE this[int index]
    {
        get { return PrivDoubleLinkedListGetNode(index).Value; }
        set
        {
            CDoubleLinkedListNode<TEMPLATE> node = PrivDoubleLinkedListGetNode(index);
            node.Value = value;
        }
    }

    //--------------------------------------------------------------------------------------
    public struct Enumerator

    {
        private CDoubleLinkedListNode<TEMPLATE> m_pNodeCurrent;
        private CDoubleLinkedListNode<TEMPLATE> m_pNodeNext;
        private CDoubleLinkedListNode<TEMPLATE> m_pNodePrevious;
        private CDoubleLinkedList<TEMPLATE> m_pLinkedListOwner;

        public TEMPLATE Current { 
            get {
                if (m_pNodeCurrent != null) return m_pNodeCurrent.Value;
                else return default(TEMPLATE);
            } 
        }

        public bool MoveNext()
        {
            bool bNext = false;
            if (m_pNodeNext != null)
            {
                m_pNodePrevious = m_pNodeCurrent;
                m_pNodeCurrent = m_pNodeNext;
                m_pNodeNext = null;
                bNext = true;
            }
            else
            {
                if (m_pNodeCurrent != null)
                {
                    m_pNodePrevious = m_pNodeCurrent;
                    m_pNodeCurrent = m_pNodeCurrent.Next;
                    if (m_pNodeCurrent != null)
                    {
                        bNext = true;
                    }
                }
            }

            return bNext;
        }

        public bool MovePrev()
        {
            bool bPrev = false;
            if (m_pNodePrevious != null)
            {
                m_pNodeNext = m_pNodeCurrent;
                m_pNodeCurrent = m_pNodePrevious;
                m_pNodePrevious = null;
                bPrev = true;
            }
            else
            {
                if (m_pNodeCurrent != null)
                {
                    m_pNodeNext = m_pNodeCurrent;
                    m_pNodeCurrent = m_pNodeCurrent.Previous;
                    if (m_pNodeCurrent != null)
                    {
                        bPrev = true;
                    }
                }
            }

            return bPrev;
        }

        public void Remove()
        {
            if (m_pLinkedListOwner == null) return;
            if (m_pNodeCurrent == null) return;
            m_pNodeNext = m_pNodeCurrent.Next;
            m_pNodePrevious = m_pNodeCurrent.Previous;
            m_pLinkedListOwner.Remove(m_pNodeCurrent);
            m_pNodeCurrent = null;
            
        }

        public void AddAfter(TEMPLATE value)
        {
            if (m_pLinkedListOwner == null) return;

            CDoubleLinkedListNode<TEMPLATE> newNode;

            if (m_pNodeCurrent == null)
            {
                newNode = PrivDoubleLinkedListEnumeratorAddCurrent(value);
            }
            else
            {
                newNode = m_pLinkedListOwner.InterAddAfter(m_pNodeCurrent, value);
            }
            m_pNodeNext = newNode;

        }

        public void AddBefore(TEMPLATE value)
        {
            if (m_pLinkedListOwner == null) return;

            CDoubleLinkedListNode<TEMPLATE> newNode;

            if (m_pNodeCurrent == null)
            {
                newNode = PrivDoubleLinkedListEnumeratorAddCurrent(value);
            }
            else
            {
                newNode = m_pLinkedListOwner.InterAddBefore(m_pNodeCurrent, value);
            }
            m_pNodePrevious = newNode;
        }

        public void Reset()
        {
            m_pNodeCurrent = m_pLinkedListOwner.First;
        }

        public bool CanUse()
        {
            return m_pLinkedListOwner != null;
        }

        public Enumerator(CDoubleLinkedList<TEMPLATE> pOwner, bool bLastEnumerator = false)
        {
            m_pLinkedListOwner = pOwner;
            m_pNodeCurrent = null;

            if (bLastEnumerator)
            {
                m_pNodeNext = null;
                m_pNodePrevious = pOwner.Last;
            }
            else
            {
                m_pNodeNext = pOwner.First;
                m_pNodePrevious = null;
            }
            
        }

        //-------------------------------------------------------------
        
        private CDoubleLinkedListNode<TEMPLATE> PrivDoubleLinkedListEnumeratorAddCurrent(TEMPLATE value)
        {
            CDoubleLinkedListNode<TEMPLATE> newNode;

            if (m_pNodePrevious != null)
            {
                newNode = m_pLinkedListOwner.InterAddAfter(m_pNodePrevious, value);
            }
            else if (m_pNodeNext != null)
            {
                newNode = m_pLinkedListOwner.InterAddBefore(m_pNodeNext, value);
            }
            else //prev, current, next 모두 null인 경우, AddFirst로 enumerator를 받아서 current를 변경한다.
            {
                Enumerator enumerator = m_pLinkedListOwner.AddFirst(value);
                newNode = enumerator.m_pNodeNext;
            }
            return newNode;
        }
    }
}

public sealed class CDoubleLinkedListNode<TEMPLATE>
{
    public CDoubleLinkedListNode(TEMPLATE value)
    {
        Value = value;
    }
    private CDoubleLinkedList<TEMPLATE> m_pOwner;                  internal void InterSetOwner(CDoubleLinkedList<TEMPLATE> owner) { m_pOwner = owner; }
    private CDoubleLinkedListNode<TEMPLATE> m_pNext;               internal void InterSetNext(CDoubleLinkedListNode<TEMPLATE> next) { m_pNext = next; } 
    private CDoubleLinkedListNode<TEMPLATE> m_pPrevious;           internal void InterSetPrevious(CDoubleLinkedListNode<TEMPLATE> previous) { m_pPrevious = previous; } 

    public CDoubleLinkedList<TEMPLATE> List { get { return m_pOwner; } }
    public CDoubleLinkedListNode<TEMPLATE> Next { get { return m_pNext; } }
    public CDoubleLinkedListNode<TEMPLATE> Previous { get { return m_pPrevious; } }
    public TEMPLATE Value { get; set; }
}