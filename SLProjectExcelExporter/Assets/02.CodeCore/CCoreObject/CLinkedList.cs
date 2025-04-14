using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 베이직 LinkedList가 이터레이팅중 삭제가 안되는 점을 보완하기 위해 구현되었다.
// 외부에서는 동일한 MoveNext패턴으로 입출력하면 되며 삭제시 List의 Remove가 아닌
// Enumerator 의 Remove를 사용하여 반복문 중에 삭제가 가능하도록 되어 있다. 

public class CLinkedList<TEMPLATE> : LinkedList<TEMPLATE> 
{
    public new Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    //--------------------------------------------------------------------
    public new struct Enumerator 
    {
        private LinkedListNode<TEMPLATE> m_pNodeCurrent;
        private LinkedListNode<TEMPLATE> m_pNodeNext;
        private CLinkedList<TEMPLATE>    m_pLinkedListOwner;

        public TEMPLATE Current { get { return m_pNodeCurrent.Value; } } 

        public bool MoveNext()
        {
            bool bNext = false;
            if (m_pNodeNext != null)
            {
                m_pNodeCurrent = m_pNodeNext;
                m_pNodeNext = null;
                bNext = true;
            }           
            else
            {
                if (m_pNodeCurrent != null)
                {
                    m_pNodeCurrent = m_pNodeCurrent.Next;
                    if (m_pNodeCurrent != null)
                    {
                        bNext = true;
                    }
                }
            }
            
            return bNext;
        }

        public void Remove() 
        {
            if (m_pLinkedListOwner == null) return;
            if (m_pNodeCurrent == null) return;

            m_pNodeNext = m_pNodeCurrent.Next;
            m_pLinkedListOwner.Remove(m_pNodeCurrent);
            
            m_pNodeCurrent = null;
        }

        public void Reset()
        {
            m_pNodeCurrent = m_pLinkedListOwner.First;
        }

        public Enumerator(CLinkedList<TEMPLATE> pOwner)
        {           
            m_pLinkedListOwner = pOwner;
            m_pNodeCurrent = null;
            m_pNodeNext = pOwner.First;
        }
    }

}
