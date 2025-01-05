"use client";

import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";

interface HistoryItem {
  id: string;
  event: string;
  data: unknown;
  timestamp: string;
}

interface HistoryListProps {
  historyItems: HistoryItem[];
}

export function HistoryList({ historyItems }: HistoryListProps) {
  const sortedItems = historyItems.sort(
    (a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
  );

  return (
    <Accordion type="single" collapsible className="w-full">
      {sortedItems.map((item, index) => (
        <AccordionItem value={`item-${index}`} key={item.id}>
          <AccordionTrigger className="text-left">
            {new Date(item.timestamp).toLocaleString()} - {item.event}
          </AccordionTrigger>
          <AccordionContent>
            <pre className="bg-gray-100 p-2 rounded-md overflow-x-auto">
              {JSON.stringify(item.data, null, 2)}
            </pre>
          </AccordionContent>
        </AccordionItem>
      ))}
    </Accordion>
  );
}
