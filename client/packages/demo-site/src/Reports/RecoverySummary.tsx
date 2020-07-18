import React from "react";
import { TinyBITable, useQuery, TinyBIChartBox } from "tinybi-react";
import { Bug, Workflow } from "../demoSchema";
import { VisualProps } from "./VisualProps";

export interface RecoverySummaryProps extends VisualProps {
    fixedByCustomer: boolean;
    title: string;
}

export function RecoverySummary({ pageFilters, fixedByCustomer, title, fetch }: RecoverySummaryProps) {

    const query = {
        select: [Workflow.WorkflowState],
        aggregations: [Bug.Id.count()],
        filters: [
            Workflow.FixedByCustomer.equalTo(fixedByCustomer),                    
        ],
        totals: true
    };

    const data = useQuery(fetch, query, pageFilters);

    return (
        <TinyBIChartBox title={title}>
            <TinyBITable
                data={data}
                columns={{
                    State: (d) => `${d.keys[0]}`,                
                    Count: (d) => [`${d.value(0)}`, "right"],
                    "% of Count": (d) => [`${d.share(0)}%`, "right"],
                }} 
            />
        </TinyBIChartBox>
    );
}