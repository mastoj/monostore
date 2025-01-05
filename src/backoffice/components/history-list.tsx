"use client";

import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { Change } from "@/lib/monostore-api/monostore-api";

interface HistoryListProps {
  historyItems: Change[];
}

export function HistoryList({ historyItems }: HistoryListProps) {
  const sortedItems = historyItems.sort(
    (a, b) => new Date(b.timeStamp).getTime() - new Date(a.timeStamp).getTime()
  );

  return (
    <Accordion type="single" collapsible className="w-full">
      {sortedItems.map((item, index) => (
        <AccordionItem value={`item-${index}`} key={item.version}>
          <AccordionTrigger className="text-left">
            {new Date(item.timeStamp).toLocaleString()} - {item.changeType}
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
